using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MaeveFramework.SDK.Plugins;
using MaeveFramework.SDK.Scheduler;
using MaeveFramework.SDK.Scheduler.Abstractions;

namespace MaeveFramework.Scheduler
{
    public class Manager
    {
        private static List<JobDetails> _jobsList;

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
                _jobsList = new List<JobDetails>();

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
                    var jobObject = _jobsList.Find(x => x.Guid == job.Key);

                    if (DateTime.Now >= job.Value && jobObject.State == JobState.Crash)
                    {
                        try
                        {
                            RunJob(_jobsList.Find(x => x.Guid == job.Key));
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, $"Exception on crashed {jobObject.Name} job restart - next restart try in {JOBRESTARTNEXTTRYMIN} minutes");

                            _jobsList.Find(x => x.Guid == job.Key).State = JobState.Crash;
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

        public static List<JobDetails> GetJobsDetailsList()
        {
            return _jobsList;
        }

        public static JobDetails GetJobDetail(string name)
        {
            return _jobsList.Find(x => x.Name.Equals(name));
        }

        public static JobDetails GetJobDetail(Guid guid)
        {
            return _jobsList.Find(x => x.Guid.Equals(guid));
        }

        public static Guid CreateJob(JobBase job, string schedule)
        {
            return CreateJob(job, ScheduleString.Parse(schedule));
        }

        /// <summary>
        /// Create new job to run
        /// </summary>
        /// <param name="job"></param>
        /// <param name="repeat">If null will be run for one time only</param>
        public static Guid CreateJob(JobBase job, Schedule schedule)
        {
            if (_jobsList.Count > 20)
                throw new ArgumentException("Can't create more then 20 jobs!");

            if (_jobsList.Exists(x => x.Name == job.Name))
                throw new ArgumentException($"Job with name {job.Name} is already exist!");

            JobDetails jobDetail = new JobDetails()
            {
                Name = job.Name,
                State = JobState.NotStarted,
                Schedule = schedule,
                Guid = Guid.NewGuid(),
                Job = job
            };
            jobDetail.NextRun = schedule.GetNextRun();
            jobDetail.JobCancelToken = new CancellationTokenSource();

            Task task = new Task(CreateAction(job, jobDetail), jobDetail.JobCancelToken.Token, TaskCreationOptions.RunContinuationsAsynchronously);
            jobDetail.SystemTask = task;

            _jobsList.Add(jobDetail);

            return jobDetail.Guid;
        }

        /// <summary>
        /// Run created jobs
        /// </summary>
        public static void RunJobs()
        {
            foreach (JobDetails job in _jobsList)
            {
                RunJob(job);
            }
        }

        public static void RunJob(Guid guid)
        {
            var job = _jobsList.Find(x => x.Guid == guid);

            RunJob(job);
        }

        private static void RunJob(JobDetails job)
        {
            if ((job.State == JobState.NotStarted || job.State == JobState.Stopped ||
                 job.State == JobState.Crash) && job.SystemTask.Status != TaskStatus.Running)
            {
                if (job.SystemTask.IsCompleted || job.SystemTask.IsFaulted)
                {
                    _logger.Warn(
                        $"Starting again stopped or crashed job, task state: {job.SystemTask.Status.ToString()}, job state: {job.StateName}");

                    job.JobCancelToken = new CancellationTokenSource();

                    Task task = new Task(CreateAction(job.Job, job), job.JobCancelToken.Token,
                        TaskCreationOptions.RunContinuationsAsynchronously);
                    job.SystemTask = task;

                    _jobsList.Insert(_jobsList.FindIndex(x => x.Guid == job.Guid), job);
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
            var job = _jobsList.Find(x => x.Guid == guid);

            StopJob(job);
        }

        private static void StopJob(JobDetails job)
        {
            if (job.SystemTask.Status != TaskStatus.Canceled)
            {
                _jobsList.Find(x => x.Guid == job.Guid).JobCancelToken.Cancel(false);
            }
        }

        private static Action CreateAction(JobBase job, JobDetails jobDetail)
        {
            Action action = new Action(() =>
            {
                Thread.CurrentThread.Name = $"MaeveFramework.Scheduler_{jobDetail.Name}";

                try
                {
                    // OnStart
                    _jobsList.Find(x => x.Guid == jobDetail.Guid).State = JobState.Starting;
                    job.OnStart();

                    _jobsList.Find(x => x.Guid == jobDetail.Guid).NextRun = jobDetail.Schedule.GetNextRun(true);

                    _logger.Debug($"Job {jobDetail.Name} started, run scheduled at {_jobsList.Find(x => x.Guid == jobDetail.Guid).NextRun}");

                    while (_jobsList.Find(x => x.Guid == jobDetail.Guid).State != JobState.Stopping || !jobDetail.JobCancelToken.Token.IsCancellationRequested)
                    {
                        bool isJobTime = (DateTime.Now >= _jobsList.Find(x => x.Guid == jobDetail.Guid).NextRun.Value);

                        if (isJobTime)
                        {
                            if (jobDetail.LastRun.GetValueOrDefault(DateTime.MinValue).AddSeconds(1) < DateTime.Now)
                            {
                                _jobsList.Find(x => x.Guid == jobDetail.Guid).LastRun = DateTime.Now;
                                _jobsList.Find(x => x.Guid == jobDetail.Guid).State = JobState.Working;

                                job.Job();

                                _jobsList.Find(x => x.Guid == jobDetail.Guid).NextRun = jobDetail.Schedule.GetNextRun();

                                _jobsList.Find(x => x.Guid == jobDetail.Guid).State = JobState.Idle;

                                _logger.Debug($"Job {jobDetail.Name} is now idle, next run at {_jobsList.Find(x => x.Guid == jobDetail.Guid).NextRun}");
                            }
                            else
                            {
                                // can't run job more often then 1 second;
                            }
                        }

                        if (jobDetail.Schedule.OneTime)
                        {
                            break;
                        }

                        Thread.Sleep(1000);

                        jobDetail.JobCancelToken.Token.ThrowIfCancellationRequested();
                    }

                    // OnStop
                    throw new OperationCanceledException("End of job");
                }
                catch (OperationCanceledException ex)
                {
                    _logger.Error(ex, $"Stopping job {jobDetail.Name}, reason: {ex.Message}");

                    _jobsList.Find(x => x.Guid == jobDetail.Guid).State = JobState.Stopping;
                    job.OnStop();

                    _jobsList.Find(x => x.Guid == jobDetail.Guid).State = JobState.Stopped;

                    if (_jobsToRestart.ContainsKey(jobDetail.Guid))
                        _jobsToRestart.Remove(jobDetail.Guid);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Unhandled job exception");

                    JobState currentState = _jobsList.Find(x => x.Guid == jobDetail.Guid).State.Value;
                    _jobsList.Find(x => x.Guid == jobDetail.Guid).State = JobState.Crash;

                    if (currentState != JobState.Stopped || currentState != JobState.Stopping)
                    {
                        DateTime restartDateTime = DateTime.Now.AddSeconds(JOBRESTARTAFTERSEC);

                        if (_jobsToRestart.ContainsKey(jobDetail.Guid))
                        {
                            restartDateTime = restartDateTime.AddMinutes(JOBRESTARTNEXTTRYMIN);

                            _jobsToRestart.Remove(jobDetail.Guid);
                            _jobsToRestart.Add(jobDetail.Guid, restartDateTime);
                        }
                        else
                        {
                            _jobsToRestart.Add(jobDetail.Guid, restartDateTime);
                        }

                        _jobRestartTimer.Start();
                    }
                }
                finally
                {
                    _logger.Debug($"Job {jobDetail.Name} is now {_jobsList.Find(x => x.Guid == jobDetail.Guid).State}");

                    _jobsList.Find(x => x.Guid == jobDetail.Guid).JobCancelToken.CancelAfter(200);
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

            foreach (var job in _jobsList)
            {
                job.State = JobState.Stopped;

                if (job.JobCancelToken.Token.CanBeCanceled)
                    job.JobCancelToken.Cancel(false);
            }
        }
    }
}
