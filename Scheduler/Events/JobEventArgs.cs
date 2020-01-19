using System;
using System.Collections.Generic;
using System.Text;

namespace MaeveFramework.Scheduler.Events
{
    public class JobEventArgs : EventArgs
    {
        public JobEventArgs(string jobName, string message)
        {
            JobName = jobName;
            Message = message;
        }

        public string JobName { get; set; }
        public string Message { get; set; }
    }
}
