using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MaeveFramework.Logger;
using MaeveFramework.Logger.Abstractions;

namespace MaeveFramework.Scheduler.Abstractions
{
    /// <summary>
    /// Job class with options abstraction
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    public abstract class JobBase<TOptions> : JobBase
    {
        /// <inheritdoc cref="JobBase{TOptions}" />
        public JobBase(Schedule schedule, TOptions options) : base(schedule, options)
        {

        }

        /// <summary>
        /// Job options object
        /// </summary>
        public new TOptions Options => (TOptions)base.Options;
    }

    /// <summary>
    /// Job base class
    /// </summary>
    public abstract class JobBase : JobOptions<object>
    {
        /// <summary>
        /// Job logger
        /// </summary>
        protected internal readonly ILogger Logger;

        /// <summary>
        /// Job abstraction type
        /// </summary>
        protected internal Type JobType => this.GetType();
        /// <summary>
        /// Job name
        /// </summary>
        public string Name => JobType.Name;
        /// <summary>
        /// Job name with namespace
        /// </summary>
        public string FullName => JobType.FullName;

        /// <inheritdoc cref="JobBase" />
        public JobBase(Schedule schedule, object options = null) : base(schedule, options)
        {
            State = JobStateEnum.NotStarted;
            Logger = LoggingManager.GetLogger(FullName);
            NextRun = Schedule.GetNext(!LastRun.HasValue);
        }

        /// <summary>
        /// Job next run time
        /// </summary>
        public DateTime NextRun { get; internal set; }
        /// <summary>
        /// Job last run time
        /// </summary>
        public DateTime? LastRun { get; internal set; }
        /// <summary>
        /// Job current state
        /// </summary>
        public JobStateEnum State
        {
            get
            {
                return _state;
            }
            internal set
            {
                _state = value;
                StateChange?.Invoke(this, value);
            }
        }
        /// <summary>
        /// Event for Job State change
        /// </summary>
        public event EventHandler<JobStateEnum> StateChange;

        private JobStateEnum _state = JobStateEnum.NotSet;

        /// <summary>
        /// Executed on Job start only after initilization of Job Controller
        /// </summary>
        public virtual void OnStart()
        {

        }

        /// <summary>
        /// Executed on schedule or wake action
        /// </summary>
        public virtual void Job()
        {

        }

        /// <summary>
        /// Executed on Job Controller STOP call
        /// </summary>
        public virtual void OnStop()
        {

        }

        /// <summary>
        /// Get Job HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode() + Guid.GetHashCode();
        }

        /// <summary>
        /// Get Job text representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name} Last run {LastRun} Next run {NextRun} State {State}";
        }
    }
}
