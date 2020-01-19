using System;
using System.Collections.Generic;
using System.Text;

namespace TachankaScheduler.Events
{
    public class ManagerEventArgs : EventArgs
    {
        public ManagerEventArgs(string message, Exception exception)
        {
            Message = message;
            Exception = exception;
        }

        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}
