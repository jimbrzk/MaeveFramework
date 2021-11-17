using MaeveFramework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MaeveFramework.Scheduler.Abstractions
{
    /// <summary>
    /// Job controller for job
    /// </summary>
    public class JobController
    {
        private readonly object _jobActionRygiel = new object();

        /// <inheritdoc cref="JobController" />
        public JobController(JobBase job)
        {
            Job = job;
        }

        /// <summary>
        /// Controled job
        /// </summary>
        public readonly JobBase Job;
        private Task JobTask { get; set; }
        private CancellationTokenSource JobCancelToken { get; set; }
        private CancellationTokenSource WaitCancelToken { get; set; }

        /// <summary>
        /// Wait for Job State
        /// </summary>
        /// <param name="acceptedStates">Required state</param>
        /// <param name="timeout">(Optional) timeout of waiting for state</param>
        /// <returns></returns>
        public bool WaitForState(JobStateEnum[] acceptedStates, TimeSpan? timeout = null)
        {
            if (acceptedStates.Contains(Job.State)) return true;
            DateTime end = DateTime.Now.Add(timeout.GetValueOrDefault(TimeSpan.MaxValue));
            bool wait = true;
            EventHandler<JobStateEnum> stateEventHandlerAction = (caller, state) =>
            {
                if (acceptedStates.Contains(state))
                    wait = false;
            };

            try
            {
                Job.StateChange += stateEventHandlerAction;

                while (wait)
                {
                    if (acceptedStates.Contains(Job.State)) return true;
                    if (end <= DateTime.Now) break;
                    Thread.Sleep(10.Miliseconds());
                }
            }
            catch (Exception ex)
            {
                Job.Logger.Error(ex, "Failed waiting for state!");
            }
            finally
            {
                Job.StateChange -= stateEventHandlerAction;
            }

            return acceptedStates.Contains(Job?.State ?? JobStateEnum.NotSet);
        }

        /// <inheritdoc cref="WaitForState(JobStateEnum[], TimeSpan?)" />
        public bool WaitForState(JobStateEnum state, TimeSpan? timeout = null) => WaitForState(new[] { state }, timeout);

        /// <summary>
        /// Start job task with configured schedule
        /// </summary>
        public void StartJob()
        {
            lock (_jobActionRygiel)
            {
                if ((Job.State == JobStateEnum.NotStarted || Job.State == JobStateEnum.Stopped || Job.State == JobStateEnum.Crash) && (JobTask?.Status ?? TaskStatus.WaitingToRun) != TaskStatus.Running)
                {
                    WaitCancelToken = new CancellationTokenSource();

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
        }

        /// <summary>
        /// Stop job
        /// </summary>
        /// <param name="force"></param>
        public void StopJob(bool force = false)
        {
            lock (_jobActionRygiel)
            {
                Job.State = JobStateEnum.Stopping;

                if (WaitCancelToken?.Token.CanBeCanceled ?? false)
                    WaitCancelToken.Cancel(false);
                if (JobCancelToken?.Token.CanBeCanceled ?? false)
                    JobCancelToken.Cancel(false);

                Job.State = JobStateEnum.Stopped;
            }
        }

        /// <summary>
        /// Is current job state allowing to wake action
        /// </summary>
        /// <returns></returns>
        public bool CanBeWake()
        {
            JobStateEnum[] invalidStates = new[] { JobStateEnum.Stopping, JobStateEnum.Starting, JobStateEnum.NotSet, JobStateEnum.Working };

            if (invalidStates.Contains(Job.State) || (WaitCancelToken?.IsCancellationRequested ?? true))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Wake job and ignore schedule
        /// </summary>
        public void Wake()
        {
            if (!CanBeWake()) return;
            if (Job.State == JobStateEnum.NotStarted) StartJob();
            if (!CanBeWake()) return;
            lock (_jobActionRygiel)
            {
                if (CanBeWake())
                {
                    Job.State = JobStateEnum.Wake;
                    WaitCancelToken?.Cancel(false);
                }
            }
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
                        Job.Logger.Trace("Unable to start Job, JobCancelToken requested!");

                    Job.State = JobStateEnum.Started;

                    if (JobTask.IsCompleted)
                        Job.Logger.Trace("Job task is completed");

                    while (!JobCancelToken.IsCancellationRequested)
                    {
                        if (Job.State == JobStateEnum.Stopping || Job.State == JobStateEnum.Stopped)
                        {
                            break;
                        }

                        Int32 waitMsInt32 = 0;
                        if (!JobCancelToken.IsCancellationRequested)
                        {
                            lock (_jobActionRygiel)
                            {
                                if (Job.Schedule.IsNow() || Job.State == JobStateEnum.Wake)
                                {
                                    try
                                    {
                                        if (Job.State == JobStateEnum.Wake)
                                            Job.Logger.Debug("Job is waking up");

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
                                        Job.NextRun = Job.Schedule.GetNext();
                                        Job.Logger.Debug($"Job {Job.Name} complete, next run: {Job.NextRun}");
                                    }
                                }

                                if (Job.State == JobStateEnum.Stopping || Job.State == JobStateEnum.Stopped || JobCancelToken.Token.IsCancellationRequested)
                                    break;

                                if (!JobCancelToken.IsCancellationRequested)
                                {
                                    if (WaitCancelToken?.IsCancellationRequested ?? true)
                                        WaitCancelToken = new CancellationTokenSource();

                                    double waitMs = Job.NextRun.Subtract(DateTime.Now).TotalMilliseconds;
                                    waitMsInt32 = (waitMs > Int32.MaxValue)
                                    ? Int32.MaxValue
                                    : Convert.ToInt32(waitMs);
                                }
                            }
                        }

                        try
                        {
                            if (!JobCancelToken.IsCancellationRequested)
                            {
                                Job.State = JobStateEnum.Idle;
                                JobTask.Wait(waitMsInt32, WaitCancelToken.Token);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (!JobCancelToken.IsCancellationRequested && !WaitCancelToken.IsCancellationRequested)
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
                    Job.Logger.Debug(ex, $"Stopping job {Job.Name}, reason: {ex.Message}");
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
