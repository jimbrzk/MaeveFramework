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

namespace MaeveFramework.Scheduler
{
    public class SchedulerManager
    {
        private static readonly List<JobController> _jobs = new List<JobController>();
        private static object _jobsLocker = new object();

        private static ILogger _logger;

        public static List<JobController> Jobs
        {
            get
            {
                lock (_jobsLocker)
                {
                    return _jobs;
                }
            }
        }

        public JobBase this[Type jobType]
        {
            get
            {
                try
                {
                    lock (_jobsLocker)
                    {
                        return _jobs.FirstOrDefault(x => x.Job.JobType == jobType)?.Job;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    return null;
                }
            }
        }

        public JobBase this[Guid jobGuid]
        {
            get
            {
                try
                {
                    lock (_jobsLocker)
                    {
                        return _jobs.FirstOrDefault(x => x.Job.Guid == jobGuid)?.Job;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    return null;
                }
            }
        }

        public static JobBase Job<JobType>() where JobType : JobBase => _jobs.FirstOrDefault(x => x.Job.JobType == typeof(JobType))?.Job as JobType;
        public static JobBase Job<JobType>(Guid jobGuid) where JobType : JobBase => _jobs.FirstOrDefault(x => x.Job.Guid == jobGuid)?.Job as JobType;

        public static JobController JobController(Guid jobGuid) => _jobs.FirstOrDefault(x => x.Job.Guid == jobGuid);
        public static JobController JobController(string jobName) => _jobs.FirstOrDefault(x => x.Job.Name == jobName);

        static SchedulerManager()
        {
            if (_logger == null)
                _logger = LoggingManager.GetLogger(nameof(SchedulerManager));
        }

        /// <summary>
        /// Create new job to run
        /// </summary>
        /// <param name="job"></param>
        /// <exception cref="ArgumentException">If job allready added</exception>
        public static Guid CreateJob(JobBase job)
        {
            if (_jobs.Any(x => x.Job.JobType == job.JobType))
                throw new ArgumentException($"Job with name {job.Name} is already exist!");

            lock (_jobsLocker)
            {
                _jobs.Add(new JobController(job));

                return job.Guid;
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

                foreach (var job in _jobs)
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

                foreach (var job in _jobs)
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
    }
}
