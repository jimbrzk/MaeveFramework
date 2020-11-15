using System;
using System.Collections.Generic;
using System.Text;

namespace MaeveFramework.Scheduler.Abstractions
{
    public enum JobStateEnum
    {
        NotSet = 0,
        Idle = 1,
        Stopped = 2,
        Stopping = 3,
        Started = 4,
        Starting = 5,
        NotStarted = 6,
        Working = 7,
        Crash = 8
    }
}
