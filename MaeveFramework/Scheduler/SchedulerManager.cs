using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MaeveFramework.Scheduler.Abstractions;
using MaeveFramework.Logger;
using MaeveFramework.Logger.Abstractions;
using System.Collections.Concurrent;

namespace MaeveFramework.Scheduler
{
    /// <summary>
    /// Job scheduler class
    /// </summary>
    public class SchedulerManager
    {
        private static readonly ConcurrentDictionary<Guid, JobController> _jobs = new ConcurrentDictionary<Guid, JobController>();
        private static object _jobsLocker = new object();

        private static ILogger _logger;

        static SchedulerManager()
        {
            if (_logger == null)
                _logger = LoggingManager.GetLogger(nameof(SchedulerManager));
        }

        /// <summary>
        /// Current Jobs controller list
        /// </summary>
        public static List<JobController> Jobs
        {
            get
            {
                lock (_jobsLocker)
                {
                    return _jobs.Values.ToList();
                }
            }
        }

        /// <summary>
        /// Get Job
        /// </summary>
        /// <param name="jobType">Job with JobBase base class</param>
        /// <returns>Job object</returns>
        public JobBase this[Type jobType]
        {
            get
            {
                try
                {
                    lock (_jobsLocker)
                    {
                        return _jobs.Values.FirstOrDefault(x => x.Job.JobType == jobType)?.Job;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// Get Job
        /// </summary>
        /// <param name="jobGuid">Job GUID</param>
        /// <returns>Job object</returns>
        public JobBase this[Guid jobGuid]
        {
            get
            {
                try
                {
                    lock (_jobsLocker)
                    {
                        return _jobs.Values.FirstOrDefault(x => x.Job.Guid == jobGuid)?.Job;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// Get job by type
        /// </summary>
        /// <typeparam name="JobType">Job with JobBase base class</typeparam>
        /// <returns>Job</returns>
        public static JobType Job<JobType>() where JobType : JobBase => _jobs.Values.FirstOrDefault(x => x.Job.JobType == typeof(JobType))?.Job as JobType;
        /// <summary>
        /// Get Job base
        /// </summary>
        /// <typeparam name="JobType"></typeparam>
        /// <returns></returns>
        public static JobBase JobBase<JobType>() where JobType : JobBase => _jobs.Values.FirstOrDefault(x => x.Job.JobType == typeof(JobType))?.Job as JobType;
        /// <summary>
        /// Get job by Guid
        /// </summary>
        /// <typeparam name="JobType">Job with JobBase base class</typeparam>
        /// <param name="jobGuid">Job GUID</param>
        /// <returns>Job object</returns>
        public static JobType Job<JobType>(Guid jobGuid) where JobType : JobBase => _jobs.Values.FirstOrDefault(x => x.Job.Guid == jobGuid)?.Job as JobType;
        /// <summary>
        /// Get Job base
        /// </summary>
        /// <param name="jobGuid"></param>
        /// <returns></returns>
        public static JobBase JobBase(Guid jobGuid) => _jobs.Values.FirstOrDefault(x => x.Job.Guid == jobGuid)?.Job;

        /// <summary>
        /// Get job controller by job type
        /// </summary>
        /// <typeparam name="JobType"></typeparam>
        /// <returns>Job controller</returns>
        public static JobController JobController<JobType>() where JobType : JobBase => _jobs.Values.FirstOrDefault(x => x.Job.JobType == typeof(JobType));
        /// <summary>
        /// Get job controller
        /// </summary>
        /// <param name="jobGuid">Job GUID</param>
        /// <returns>Job controller object with job object</returns>
        public static JobController JobController(Guid jobGuid) => _jobs.Values.FirstOrDefault(x => x.Job.Guid == jobGuid);
        /// <summary>
        /// Get job controller
        /// </summary>
        /// <param name="jobName">Job Name string</param>
        /// <returns>Job controller object with job object</returns>
        public static JobController JobController(string jobName) => _jobs.Values.FirstOrDefault(x => x.Job.Name == jobName);

        /// <summary>
        /// Create new job to run
        /// </summary>
        /// <param name="job"></param>
        /// <exception cref="ArgumentException">If job allready added</exception>
        public static Guid CreateJob(JobBase job)
        {
            if (_jobs.Values.Any(x => x.Job.JobType == job.JobType))
                throw new ArgumentException($"Job with name {job.Name} is already exist!");

            lock (_jobsLocker)
            {
                return _jobs.GetOrAdd(job.Guid, new JobController(job)).Job.Guid;
            }
        }

        /// <summary>
        /// Run created jobs
        /// </summary>
        public static void StartAllJobs()
        {
            lock (_jobsLocker)
            {
                _logger.Debug("Starting all jobs");

                foreach (var job in _jobs.Values)
                {
                    job.StartJob();
                }
            }
        }

        /// <summary>
        /// Start job with given Guid
        /// </summary>
        /// <param name="jobGuid"></param>
        /// <exception cref="NullReferenceException">If job not exist</exception>
        public static void StartJob(Guid jobGuid)
        {
            lock (_jobsLocker)
            {
                JobController(jobGuid).StartJob();
            }
        }

        /// <summary>
        /// Start job with given name
        /// </summary>
        /// <param name="jobName"></param>
        /// <exception cref="NullReferenceException">If job not exist</exception>
        public static void StartJob(string jobName)
        {
            lock (_jobsLocker)
            {
                JobController(jobName).StartJob();
            }
        }

        /// <summary>
        /// Stop all jobs
        /// </summary>
        public static void StopAllJobs(bool force = false)
        {
            lock (_jobsLocker)
            {
                _logger.Debug("Stopping all jobs");

                foreach (var job in _jobs.Values)
                {
                    job.StopJob(force);
                }
            }
        }

        /// <summary>
        /// Stop job with given Guid
        /// </summary>
        /// <param name="jobGuid"></param>
        public static void StopJob(Guid jobGuid)
        {
            lock (_jobsLocker)
            {
                JobController(jobGuid).StopJob();
            }
        }

        /// <summary>
        /// Stop job with given name
        /// </summary>
        /// <param name="jobName"></param>
        /// <exception cref="NullReferenceException">If job not exist</exception>
        public static void StopJob(string jobName)
        {
            lock (_jobsLocker)
            {
                JobController(jobName).StopJob();
            }
        }

        public static void RemoveJob(string jobName)
        {
            lock (_jobsLocker)
            {
                if (!_jobs.Values.Any(x => x.Job.Name == jobName)) return;
            }

            StopJob(jobName);

            lock (_jobsLocker)
            {
                var jobController = _jobs.Values.FirstOrDefault(x => x.Job.Name == jobName);
                if (jobController != null) _jobs.TryRemove(jobController.Job.Guid, out jobController);
            }
        }

        /// <summary>
        /// Stop and remove job
        /// </summary>
        /// <param name="jobGuid"></param>
        public static void RemoveJob(Guid jobGuid)
        {
            lock (_jobsLocker)
            {
                if (!_jobs.Values.Any(x => x.Job.Guid == jobGuid)) return;
            }

            StopJob(jobGuid);

            lock (_jobsLocker)
            {
                _jobs.TryRemove(jobGuid, out JobController removed);
            }
        }
    }
}
