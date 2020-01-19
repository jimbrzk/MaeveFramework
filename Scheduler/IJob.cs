using System;
using System.Collections.Generic;
using System.Text;

namespace TachankaScheduler
{
    public interface IJob
    {
        void OnStart();

        void Job();

        void OnStop();
    }
}
