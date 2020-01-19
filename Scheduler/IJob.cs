using System;
using System.Collections.Generic;
using System.Text;

namespace MaeveFramework.Scheduler
{
    public interface IJob
    {
        void OnStart();

        void Job();

        void OnStop();
    }
}
