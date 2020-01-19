using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MaeveFramework.Scheduler.Events;

namespace MaeveFramework.Scheduler
{
    public class Manager
    {
        public static event EventHandler<Events.ManagerEventArgs> ManagerEvent;

        private static List<JobObject.JobDetails> _jobsList;

        private static Dictionary<Guid, DateTime> _jobsToRestart;
        private static System.Timers.Timer _jobRestartTimer;

        public const int JOBRESTARTAFTERSEC = 10;
        public const int JOBRESTARTNEXTTRYMIN = 5;
        public const TaskCreationOptions TASKOPTION = TaskCreationOptions.RunContinuationsAsynchronously;

        static Manager()
        {
            if (_jobsList == null)
                _jobsList = new List<JobObject.JobDetails>();

            if (_jobsToRestart == null)
                _jobsToRestart = new Dictionary<Guid, DateTime>();

            if(_jobRestartTimer == null)
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

        private static void OnManagerEvent(object sender, ManagerEventArgs e)
        {
            var handler = ManagerEvent;
            if (handler != null)
                handler(typeof(Manager), e);
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

                    if (DateTime.Now >= job.Value && jobObject.State == JobObject.JobState.Crash)
                    {
                        try
                        {
                            RunJob(_jobsList.Find(x => x.Guid == job.Key));
                        }
                        catch (Exception ex)
                        {
                            OnManagerEvent(null, new ManagerEventArgs($"Exception on crashed {jobObject.Name} job restart - next restart try in {JOBRESTARTNEXTTRYMIN} minutes", ex));

                            _jobsList.Find(x => x.Guid == job.Key).State = JobObject.JobState.Crash;
                            _jobsToRestart[job.Key] = DateTime.Now.AddMinutes(JOBRESTARTNEXTTRYMIN);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                OnManagerEvent(null, new ManagerEventArgs("Unhandled job restarter exception", ex));
            }
        }

        public static List<JobObject.JobDetails> GetJobsDetailsList()
        {
            return _jobsList;
        }

        public static JobObject.JobDetails GetJobDetail(string name)
        {
            return _jobsList.Find(x => x.Name.Equals(name));
        }

        public static JobObject.JobDetails GetJobDetail(Guid guid)
        {
            return _jobsList.Find(x => x.Guid.Equals(guid));
        }

        /// <summary>
        /// Create new job to run
        /// </summary>
        /// <param name="job"></param>
        /// <param name="repeat">If null will be run for one time only</param>
        public static void CreateJob(Type job, JobObject.Schedule schedule)
        {
            foreach (var method in typeof(IJob).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                if (job.GetMethod(method.Name) == null)
                    throw new NotImplementedException($"Can't find {method.Name} method in {job.Name} job. You must implement IJob interface with public instances in your class.");
            }

            if (_jobsList.Count > 20)
                throw new ArgumentException("Can't create more then 20 jobs!");

            if (_jobsList.Exists(x => x.Name == job.Name))
                throw new ArgumentException($"Job with name {job.Name} is already exist!");

            JobObject.JobDetails jobDetail = new JobObject.JobDetails()
            {
                Name = job.Name,
                State = JobObject.JobState.NotStarted,
                Schedule = schedule,
                Guid = Guid.NewGuid(),
                JobType = job
            };
            jobDetail.NextRun = schedule.GetNextRun();
            jobDetail.JobCancelToken = new CancellationTokenSource();

            Task task = new Task(CreateAction(job, jobDetail), jobDetail.JobCancelToken.Token, TASKOPTION);
            jobDetail.SystemTask = task;

            _jobsList.Add(jobDetail);
        }

        /// <summary>
        /// Run created jobs
        /// </summary>
        public static void RunJobs()
        {
            foreach (JobObject.JobDetails job in _jobsList)
            {
                RunJob(job);
            }
        }

        public static void RunJob(Guid guid)
        {
            var job = _jobsList.Find(x => x.Guid == guid);

            RunJob(job);
        }

        private static void RunJob(JobObject.JobDetails job)
        {
            if ((job.State == JobObject.JobState.NotStarted || job.State == JobObject.JobState.Stopped || job.State == JobObject.JobState.Crash) && job.SystemTask.Status != TaskStatus.Running)
            {
                if(job.SystemTask.IsCompleted || job.SystemTask.IsFaulted)
                {
                    OnManagerEvent(null, new ManagerEventArgs($"Starting again stopped or crashed job, task stete: {job.SystemTask.Status.ToString()}, job state: {job.StateName}", null));

                    job.JobCancelToken = new CancellationTokenSource();

                    Task task = new Task(CreateAction(job.JobType, job), job.JobCancelToken.Token, TASKOPTION);
                    job.SystemTask = task;

                    _jobsList.Insert(_jobsList.FindIndex(x => x.Guid == job.Guid), job);
                }
                
                job.SystemTask.Start();
            }
            else
            {
                OnManagerEvent(null, new ManagerEventArgs($"Cannot start job with state: {job.StateName}", null));
            }
        }

        public static void StopJob(Guid guid)
        {
            var job = _jobsList.Find(x => x.Guid == guid);

            StopJob(job);
        }

        private static void StopJob(JobObject.JobDetails job)
        {
            if(job.SystemTask.Status != TaskStatus.Canceled)
            {
                _jobsList.Find(x => x.Guid == job.Guid).JobCancelToken.Cancel(false);
            }
        }

        private static Action CreateAction(Type job, JobObject.JobDetails jobDetail)
        {
            Action action = new Action(() =>
            {
                Thread.CurrentThread.Name = $"TaskScheduler_{jobDetail.Name}";

                object instance = Activator.CreateInstance(job, null);

                try
                {
                    // OnStart
                    _jobsList.Find(x => x.Guid == jobDetail.Guid).State = JobObject.JobState.Starting;
                    job.GetMethod("OnStart").Invoke(instance, null);

                    _jobsList.Find(x => x.Guid == jobDetail.Guid).NextRun = jobDetail.Schedule.GetNextRun(true);

                    OnManagerEvent(null, new ManagerEventArgs($"Job {jobDetail.Name} started, run scheduled at {_jobsList.Find(x => x.Guid == jobDetail.Guid).NextRun}", null));

                    while (_jobsList.Find(x => x.Guid == jobDetail.Guid).State != JobObject.JobState.Stopping || !jobDetail.JobCancelToken.Token.IsCancellationRequested)
                    {
                        bool isJobTime = (DateTime.Now >= _jobsList.Find(x => x.Guid == jobDetail.Guid).NextRun.Value);

                        if (isJobTime)
                        {
                            if (jobDetail.LastRun.GetValueOrDefault(DateTime.MinValue).AddSeconds(1) < DateTime.Now)
                            {
                                _jobsList.Find(x => x.Guid == jobDetail.Guid).LastRun = DateTime.Now;
                                _jobsList.Find(x => x.Guid == jobDetail.Guid).State = JobObject.JobState.Working;

                                job.GetMethod("Job").Invoke(instance, null);

                                _jobsList.Find(x => x.Guid == jobDetail.Guid).NextRun = jobDetail.Schedule.GetNextRun();

                                _jobsList.Find(x => x.Guid == jobDetail.Guid).State = JobObject.JobState.Idle;

                                OnManagerEvent(null, new ManagerEventArgs($"Job {jobDetail.Name} is now idle, next run at {_jobsList.Find(x => x.Guid == jobDetail.Guid).NextRun}", null));
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
                catch(OperationCanceledException ex)
                {
                    OnManagerEvent(null, new ManagerEventArgs($"Stopping job {jobDetail.Name}, reasone: {ex.Message}", ex));

                    _jobsList.Find(x => x.Guid == jobDetail.Guid).State = JobObject.JobState.Stopping;
                    job.GetMethod("OnStop").Invoke(instance, null);

                    _jobsList.Find(x => x.Guid == jobDetail.Guid).State = JobObject.JobState.Stopped;

                    if (_jobsToRestart.ContainsKey(jobDetail.Guid))
                        _jobsToRestart.Remove(jobDetail.Guid);
                }
                catch (Exception ex)
                {
                    OnManagerEvent(null, new ManagerEventArgs("Unhandled job exception", ex));

                    JobObject.JobState currentState = _jobsList.Find(x => x.Guid == jobDetail.Guid).State.Value;
                    _jobsList.Find(x => x.Guid == jobDetail.Guid).State = JobObject.JobState.Crash;

                    if (currentState != JobObject.JobState.Stopped || currentState != JobObject.JobState.Stopping)
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
                    OnManagerEvent(null, new ManagerEventArgs($"Job {jobDetail.Name} is now {_jobsList.Find(x => x.Guid == jobDetail.Guid).State}", null));

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
            OnManagerEvent(null, new ManagerEventArgs("Stopping all jobs!", null));

            foreach (var job in _jobsList)
            {
                job.State = JobObject.JobState.Stopped;

                if (job.JobCancelToken.Token.CanBeCanceled)
                    job.JobCancelToken.Cancel(false);
            }
        }
    }
}
