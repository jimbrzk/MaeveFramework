using System;
using System.Collections.Generic;
using System.Text;

namespace MaeveFramework.Logger.Abstractions
{
    public interface ILogger
    {
        void Error(Exception ex);
        void Error(Exception ex, string message);
        void Error(string message);
        void Warn(Exception ex);
        void Warn(Exception ex, string message);
        void Warn(string message);
        void Debug(Exception ex);
        void Debug(Exception ex, string message);
        void Debug(string message);
        void Trace(Exception ex);
        void Trace(Exception ex, string message);
        void Trace(string message);
        void Info(Exception ex);
        void Info(Exception ex, string message);
        void Info(string message);

        LoggingLevelEnum? SelectedLevel { get; }
        bool IsDebugEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsTraceEnabled { get; }
        bool IsWarnEnabled { get; }
    }
}
