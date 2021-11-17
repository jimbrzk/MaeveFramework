using System;
using System.Collections.Generic;
using System.Text;
using MaeveFramework.Scheduler.Abstractions;

namespace MaeveFramework.Scheduler.Abstractions
{
    /// <summary>
    /// Job options
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    public abstract class JobOptions<TOptions>
    {
        /// <summary>
        /// Job options constructor
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="options"></param>
        public JobOptions(Schedule schedule, TOptions options)
        {
            Schedule = schedule;
            Options = options;
            Guid = Guid.NewGuid();
        }

        /// <summary>
        /// Job Guid
        /// </summary>
        public readonly Guid Guid;
        /// <summary>
        /// Job Schedule
        /// </summary>
        public readonly Schedule Schedule;
        /// <summary>
        /// Options abstraction type
        /// </summary>
        public readonly TOptions Options;
    }
}

