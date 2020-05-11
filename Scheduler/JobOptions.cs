using System;
using System.Collections.Generic;
using System.Text;
using MaeveFramework.Scheduler.Abstractions;
using Microsoft.Extensions.Options;

namespace MaeveFramework.Scheduler
{
    public class JobOptions
    {
        public JobState State { get; set; }
        public Guid Guid { get; protected set; }

        public Schedule Schedule { get; protected set; }
        public object Options { get; protected set; }
    }
}

