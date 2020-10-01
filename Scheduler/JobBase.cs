using System;
using System.Collections.Generic;
using System.Text;
using MaeveFramework.Logger;
using MaeveFramework.Scheduler.Abstractions;

namespace MaeveFramework.Scheduler
{
    public abstract class JobBase : JobOptions
    {
        public ILogger Logger;
        public IServiceProvider ServiceProvider;

        public string Name => this.GetType().Name;
        public Schedule Schedule => base.Schedule;

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        protected JobBase(Schedule schedule, object options = null) : base()
        {
            base.Guid = Guid.NewGuid();
            base.State = JobState.NotStarted;
            base.Options = options;
            base.Schedule = schedule;
        }

        public TTypeOptions GetJobOptions<TTypeOptions>() => (TTypeOptions)base.Options;

        public virtual void OnStart()
        {
            base.State = JobState.Starting;
        }

        public virtual void Job()
        {
            base.State = JobState.Working;
        }

        public virtual void OnStop()
        {
            base.State = JobState.Stopping;
        }
    }
}
