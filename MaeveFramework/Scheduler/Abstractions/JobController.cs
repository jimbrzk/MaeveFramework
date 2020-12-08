using MaeveFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MaeveFramework.Scheduler.Abstractions
{
    public class JobController
    {
        private readonly object _jobActionRygiel = new object();

        public JobController(JobBase job)
        {
            Job = job;
        }

        public readonly JobBase Job;
        public Task JobTask { get; private set; }
        public CancellationTokenSource JobCancelToken { get; private set; }

        public void StartJob()
        {
            if ((Job.State == JobStateEnum.NotStarted || Job.State == JobStateEnum.Stopped || Job.State == JobStateEnum.Crash) && (JobTask?.Status ?? TaskStatus.WaitingToRun) != TaskStatus.Running)
            {
                if (JobTask == null || JobTask.IsCompleted || JobTask.IsFaulted)
                {
                    JobCancelToken = new CancellationTokenSource();
                    JobTask = new Task(CreateAction(Job), JobCancelToken.Token, TaskCreationOptions.LongRunning);
                }

                JobTask.Start();
            }
            else
            {
                Job.Logger.Warn($"Cannot start job with state: {Job.State}");
            }
        }

        public void StopJob(bool force = false)
        {
            if (JobCancelToken.Token.CanBeCanceled)
                JobCancelToken.Cancel(false);

            Job.State = JobStateEnum.Stopped;
        }

        private Action CreateAction(JobBase job)
        {
            Action action = new Action(() =>
            {
                Thread.CurrentThread.Name = $"MaeveFramework.Scheduler_{Job.Name}";

                try
                {
                    Job.State = JobStateEnum.NotStarted;

                    // OnStart
                    // Restart job if exception will be throwen in OnStart
                    Job.State = JobStateEnum.Starting;
                    Job.OnStart();
                    Job.Logger.Debug($"Job {Job.Name} started, run scheduled at {Job.NextRun}");

                    if (JobCancelToken.Token.IsCancellationRequested)
                        Job.Logger.Warn("Unable to start Job, JobCancelToken requested!");

                    Job.State = JobStateEnum.Started;

                    while (!JobCancelToken.Token.IsCancellationRequested)
                    {
                        if (Job.State == JobStateEnum.Stopping || Job.State == JobStateEnum.Stopped)
                            break;

                        lock (_jobActionRygiel)
                        {
                            if (Job.Schedule.CanRun())
                            {
                                try
                                {
                                    // Job
                                    Job.State = JobStateEnum.Working;
                                    job.LastRun = DateTime.Now;
                                    Job.Job();
                                }
                                catch (Exception ex)
                                {
                                    Job.Logger.Error(ex, $"Exception on execution of job: {Job.Name}");
                                }
                                finally
                                {
                                    Job.NextRun = Job.Schedule.GetNextRun();
                                    Job.Logger.Debug($"Job {Job.Name} complete, next run: {Job.NextRun}");
                                }
                            }

                            if (Job.State == JobStateEnum.Stopping || Job.State == JobStateEnum.Stopped || JobCancelToken.Token.IsCancellationRequested)
                                break;

                            try
                            {
                                Job.State = JobStateEnum.Idle;

                                double waitMs = Job.NextRun.Subtract(DateTime.Now).TotalMilliseconds;
                                Int32 waitMsInt32 = (waitMs > Int32.MaxValue)
                                ? Int32.MaxValue
                                : Convert.ToInt32(waitMs);

                                JobCancelToken.Token.WaitHandle.WaitOne(waitMsInt32);
                                JobCancelToken.Token.WaitHandle.WaitOne(200);
                            }
                            catch (Exception ex)
                            {
                                Job.Logger.Error(ex, "Exception on waiting handle, waiting 3 seconds.");
                                JobCancelToken.Token.WaitHandle.WaitOne(3.Seconds());
                            }
                        }
                    }

                    // OnStop
                    throw new OperationCanceledException("End of job");
                }
                catch (OperationCanceledException ex)
                {
                    Job.Logger.Warn(ex, $"Stopping job {Job.Name}, reason: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Job.Logger.Error(ex, "Unhandled job exception");
                    Job.State = JobStateEnum.Crash;
                }
                finally
                {
                    try
                    {
                        if (Job.State != JobStateEnum.Crash)
                        {
                            Job.State = JobStateEnum.Stopping;
                            Job.OnStop();
                        }
                    }
                    catch (Exception ex)
                    {
                        Job.Logger.Error(ex, "Exception on stopping job");
                    }

                    Job.Logger.Debug($"Job {Job.Name} is now {Job.State}");
                    if (Job.State == JobStateEnum.Crash)
                    {
                        JobCancelToken = new CancellationTokenSource();

                        Task.Run(() =>
                        {
                            try
                            {
                                Job.Logger.Warn("Restarting job after crash in 3 seconds.");

                                JobCancelToken.Token.WaitHandle.WaitOne(3.Seconds());
                                if (!JobCancelToken.IsCancellationRequested)
                                    StartJob();
                            }
                            catch (Exception ex)
                            {
                                Job.Logger.Error(ex, "Failed to restart Job");
                            }
                        });
                    }
                    else
                    {
                        Job.State = JobStateEnum.Stopped;
                    }
                }
            });

            return action;
        }
    }
}
