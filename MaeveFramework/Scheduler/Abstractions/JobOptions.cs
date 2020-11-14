using System;
using System.Collections.Generic;
using System.Text;
using MaeveFramework.Scheduler.Abstractions;

namespace MaeveFramework.Scheduler.Abstractions
{
    public abstract class JobOptions<TOptions>
    {
        public JobOptions(Schedule schedule, TOptions options)
        {
            Schedule = schedule;
            Options = options;
            Guid = Guid.NewGuid();
        }

        public readonly Guid Guid;
        public readonly Schedule Schedule;
        public readonly TOptions Options;
    }
}

