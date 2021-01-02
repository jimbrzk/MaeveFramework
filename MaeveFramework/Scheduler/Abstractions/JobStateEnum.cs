using System;
using System.Collections.Generic;
using System.Text;

namespace MaeveFramework.Scheduler.Abstractions
{
    /// <summary>
    /// Job states
    /// </summary>
    public enum JobStateEnum
    {
        /// <summary>
        /// Job is waking up
        /// </summary>
        Wake = -1,
        /// <summary>
        /// No status has bean set
        /// </summary>
        NotSet = 0,
        /// <summary>
        /// Job witing for next run
        /// </summary>
        Idle = 1,
        /// <summary>
        /// Job is stopped
        /// </summary>
        Stopped = 2,
        /// <summary>
        /// Job is on stopping stage
        /// </summary>
        Stopping = 3,
        /// <summary>
        /// Job is started and witing for job
        /// </summary>
        Started = 4,
        /// <summary>
        /// Job is starting
        /// </summary>
        Starting = 5,
        /// <summary>
        /// Job is created but not started
        /// </summary>
        NotStarted = 6,
        /// <summary>
        /// Job is executing
        /// </summary>
        Working = 7,
        /// <summary>
        /// Job is crashed because of application exception
        /// </summary>
        Crash = 8
    }
}
