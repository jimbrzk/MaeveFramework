using MaeveFramework.Logger.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaeveFramework.Logger
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class NullLogger : LogLevel, ILogger
    {
        public NullLogger(string loggerName = null, LoggingLevelEnum? logLevel = null) { }

        public void Debug(Exception ex)
        {
            return;
        }

        public void Debug(Exception ex, string message)
        {
            return;
        }

        public void Debug(string message)
        {
            return;
        }

        public void Error(Exception ex)
        {
            return;
        }

        public void Error(Exception ex, string message)
        {
            return;
        }

        public void Error(string message)
        {
            return;
        }

        public void Info(Exception ex)
        {
            return;
        }

        public void Info(Exception ex, string message)
        {
            return;
        }

        public void Info(string message)
        {
            return;
        }

        public void Trace(Exception ex)
        {
            return;
        }

        public void Trace(Exception ex, string message)
        {
            return;
        }

        public void Trace(string message)
        {
            return;
        }

        public void Warn(Exception ex)
        {
            return;
        }

        public void Warn(Exception ex, string message)
        {
            return;
        }

        public void Warn(string message)
        {
            return;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
