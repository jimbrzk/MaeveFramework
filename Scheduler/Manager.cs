using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MaeveFramework.Plugins;
using MaeveFramework.Scheduler;
using MaeveFramework.Scheduler.Abstractions;

namespace MaeveFramework.Scheduler
{
    public class Manager
    {
        private static Dictionary<Guid, JobDetails> _jobsList;

        public static List<JobDetails> Jobs => _jobsList.Values?.ToList();

        private static ILogger _logger;
        private static Dictionary<Guid, DateTime> _jobsToRestart;
        private static System.Timers.Timer _jobRestartTimer;

        public const int JOBRESTARTAFTERSEC = 10;
        public const int JOBRESTARTNEXTTRYMIN = 5;

        static Manager()
        {
            if (_logger == null)
                _logger = LogManager.CreateLogger("MaeveFramework.Scheduler");

            if (_jobsList == null)
                _jobsList = new Dictionary<Guid, JobDetails>();

            if (_jobsToRestart == null)
                _jobsToRestart = new Dictionary<Guid, DateTime>();

            if (_jobRestartTimer == null)
            {
                _jobRestartTimer = new System.Timers.Timer
                {
                    AutoReset = true,
                    Enabled = true,
                    Interval = 2000
                };

                _jobRestartTimer.Elapsed += _jobRestartTimer_Elapsed;
            }
        }

        private static void _jobRestartTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (_jobsToRestart.Count == 0)
                    _jobRestartTimer.Stop();

                foreach (var job in _jobsToRestart)
                {
                    var jobObject = _jobsList[job.Key];

                    if (DateTime.Now >= job.Value && jobObject.Job.State == JobState.Crash)
                    {
                        try
                        {
                            RunJob(jobObject);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, $"Exception on crashed {jobObject.Job.Name} job restart - next restart try in {JOBRESTARTNEXTTRYMIN} minutes");

                            _jobsList[job.Key].Job.State = JobState.Crash;
                            _jobsToRestart[job.Key] = DateTime.Now.AddMinutes(JOBRESTARTNEXTTRYMIN);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unhandled job restarter exception");
            }
        }

        /// <summary>
        /// Create new job to run
        /// </summary>
        /// <param name="job"></param>
        public static Guid CreateJob(JobBase job)
        {
            if (_jobsList.Values.Any(x => x.Job.Name.Equals(job.Name, StringComparison.InvariantCultureIgnoreCase)))
                throw new ArgumentException($"Job with name {job.Name} is already exist!");

            var jobDetail = new JobDetails(job);
            jobDetail.NextRun = job.Schedule.GetNextRun();
            jobDetail.JobCancelToken = new CancellationTokenSource();

            Task task = new Task(CreateAction(job, jobDetail), jobDetail.JobCancelToken.Token, TaskCreationOptions.RunContinuationsAsynchronously);
            jobDetail.SystemTask = task;

            _jobsList.Add(job.Guid, jobDetail);

            return jobDetail.Job.Guid;
        }

        /// <summary>
        /// Run created jobs
        /// </summary>
        public static void RunJobs()
        {
            foreach (JobDetails job in _jobsList.Values)
            {
                RunJob(job);
            }
        }

        public static void RunJob(Guid guid)
        {
            var job = _jobsList[guid];

            RunJob(job);
        }

        private static void RunJob(JobDetails job)
        {
            if ((job.Job.State == JobState.NotStarted || job.Job.State == JobState.Stopped ||
                 job.Job.State == JobState.Crash) && job.SystemTask.Status != TaskStatus.Running)
            {
                if (job.SystemTask.IsCompleted || job.SystemTask.IsFaulted)
                {
                    _logger.Warn(
                        $"Starting again stopped or crashed job, task state: {job.SystemTask.Status.ToString()}, job state: {job.StateName}");

                    job.JobCancelToken = new CancellationTokenSource();

                    Task task = new Task(CreateAction(job.Job, job), job.JobCancelToken.Token,
                        TaskCreationOptions.RunContinuationsAsynchronously);
                    job.SystemTask = task;

                    _jobsList[job.Job.Guid] = job;
                }

                job.SystemTask.Start();
            }
            else
            {
                _logger.Warn($"Cannot start job with state: {job.StateName}");
            }
        }

        public static void StopJob(Guid guid)
        {
            var job = _jobsList[guid];
            job.JobCancelToken?.Cancel(false);
        }

        private static Action CreateAction(JobBase job, JobDetails jobDetail)
        {
            Action action = new Action(() =>
            {
                Thread.CurrentThread.Name = $"MaeveFramework.Scheduler_{jobDetail.Job.Name}";

                try
                {
                    // OnStart
                    _jobsList[job.Guid].Job.State = JobState.Starting;
                    job.OnStart();

                    _jobsList[job.Guid].NextRun = jobDetail.Job.Schedule.GetNextRun(true);

                    _logger.Debug($"Job {jobDetail.Job.Name} started, run scheduled at {_jobsList[job.Guid].NextRun}");

                    while (_jobsList[job.Guid].Job.State != JobState.Stopping || !jobDetail.JobCancelToken.Token.IsCancellationRequested)
                    {
                        bool isJobTime = (DateTime.Now >= _jobsList[job.Guid].NextRun.Value);

                        if (isJobTime)
                        {
                            if (jobDetail.LastRun.GetValueOrDefault(DateTime.MinValue).AddSeconds(1) < DateTime.Now)
                            {
                                _jobsList[job.Guid].LastRun = DateTime.Now;
                                _jobsList[job.Guid].Job.State = JobState.Working;

                                job.Job();

                                _jobsList[job.Guid].NextRun = jobDetail.Job.Schedule.GetNextRun();

                                _jobsList[job.Guid].Job.State = JobState.Idle;

                                _logger.Debug($"Job {jobDetail.Job.Name} is now idle, next run at {_jobsList[job.Guid].NextRun}");
                            }
                            else
                            {
                                // can't run job more often then 1 second;
                            }
                        }

                        jobDetail.JobCancelToken.Token.WaitHandle.WaitOne(jobDetail.NextRun.Value.Subtract(DateTime.Now));
                    }

                    // OnStop
                    throw new OperationCanceledException("End of job");
                }
                catch (OperationCanceledException ex)
                {
                    _logger.Error(ex, $"Stopping job {jobDetail.Job.Name}, reason: {ex.Message}");

                    _jobsList[job.Guid].Job.State = JobState.Stopping;
                    job.OnStop();

                    _jobsList[job.Guid].Job.State = JobState.Stopped;

                    if (_jobsToRestart.ContainsKey(jobDetail.Job.Guid))
                        _jobsToRestart.Remove(jobDetail.Job.Guid);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Unhandled job exception");

                    JobState currentState = _jobsList[job.Guid].Job.State;
                    _jobsList[job.Guid].Job.State = JobState.Crash;

                    if (currentState != JobState.Stopped || currentState != JobState.Stopping)
                    {
                        DateTime restartDateTime = DateTime.Now.AddSeconds(JOBRESTARTAFTERSEC);

                        if (_jobsToRestart.ContainsKey(jobDetail.Job.Guid))
                        {
                            restartDateTime = restartDateTime.AddMinutes(JOBRESTARTNEXTTRYMIN);

                            _jobsToRestart.Remove(jobDetail.Job.Guid);
                            _jobsToRestart.Add(jobDetail.Job.Guid, restartDateTime);
                        }
                        else
                        {
                            _jobsToRestart.Add(jobDetail.Job.Guid, restartDateTime);
                        }

                        _jobRestartTimer.Start();
                    }
                }
                finally
                {
                    _logger.Debug($"Job {jobDetail.Job.Name} is now {_jobsList[job.Guid].Job.State}");

                    _jobsList[job.Guid].JobCancelToken.CancelAfter(200);
                }
            });

            return action;
        }

        /// <summary>
        /// Cancle all jobs
        /// </summary>
        public static void CancelJobs()
        {
            _logger.Warn("Stopping all jobs");

            foreach (var job in _jobsList.Values)
            {
                job.Job.State = JobState.Stopped;

                if (job.JobCancelToken.Token.CanBeCanceled)
                    job.JobCancelToken.Cancel(false);
            }
        }

        public static ILogger GetJobLogger<T>()
        {
            return _jobsList.Values.FirstOrDefault(x => x.Job.GetType().Name == typeof(T).Name).Job.Logger;
        }

        public static T GetJobOptions<T>(string jobName)
        {
            return _jobsList.Values.FirstOrDefault(x => x.Job.Options.GetType().Name == typeof(T).Name && x.Job.Name == jobName).Job.GetJobOptions<T>();
        }
    }
}
