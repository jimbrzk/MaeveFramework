using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MaeveFramework.Logger;
using MaeveFramework.Logger.Abstractions;

namespace MaeveFramework.Scheduler.Abstractions
{
    public abstract class JobBase<TOptions> : JobBase
    {
        public JobBase(Schedule schedule, TOptions options) : base(schedule, options)
        {

        }

        public new TOptions Options => (TOptions)base.Options;
    }

    public abstract class JobBase : JobOptions<object>
    {
        public readonly ILogger Logger;

        public Type JobType => this.GetType();
        public string Name => JobType.Name;
        public string FullName => JobType.FullName;

        public JobBase(Schedule schedule, object options = null) : base(schedule, options)
        {
            State = JobStateEnum.NotStarted;
            Logger = LoggingManager.GetLogger(FullName);
        }

        public DateTime NextRun => Schedule.GetNextRun(!LastRun.HasValue);
        public DateTime? LastRun { get; private set; }
        public JobStateEnum State { get; set; }

        public virtual void OnStart()
        {
            State = JobStateEnum.Starting;
        }

        public virtual void Job()
        {
            State = JobStateEnum.Working;
            LastRun = DateTime.Now;
        }

        public virtual void OnStop()
        {
            State = JobStateEnum.Stopping;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name} Last run {LastRun} Next run {NextRun} State {State}";
        }
    }
}
