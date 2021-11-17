using System;
using System.Collections.Generic;
using System.Text;

namespace MaeveFramework.Logger.Abstractions
{
    /// <summary>
    /// Logger interface
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Method for Error level
        /// </summary>
        /// <param name="ex"></param>
        void Error(Exception ex);
        /// <summary>
        /// Method for Error level
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        void Error(Exception ex, string message);
        /// <summary>
        /// Method for Error level
        /// </summary>
        /// <param name="message"></param>
        void Error(string message);
        /// <summary>
        /// Method for Warn level
        /// </summary>
        /// <param name="ex"></param>
        void Warn(Exception ex);
        /// <summary>
        /// Method for Warn level
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        void Warn(Exception ex, string message);
        /// <summary>
        /// Method for Warn level
        /// </summary>
        /// <param name="message"></param>
        void Warn(string message);
        /// <summary>
        /// Method for Debug level
        /// </summary>
        /// <param name="ex"></param>
        void Debug(Exception ex);
        /// <summary>
        /// Method for Debug level
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        void Debug(Exception ex, string message);
        /// <summary>
        /// Method for Debug level
        /// </summary>
        /// <param name="message"></param>
        void Debug(string message);
        /// <summary>
        /// Method for Trace level
        /// </summary>
        /// <param name="ex"></param>
        void Trace(Exception ex);
        /// <summary>
        /// Method for Trace level
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        void Trace(Exception ex, string message);
        /// <summary>
        /// Method for Trace level
        /// </summary>
        /// <param name="message"></param>
        void Trace(string message);
        /// <summary>
        /// Method for Info level
        /// </summary>
        /// <param name="ex"></param>
        void Info(Exception ex);
        /// <summary>
        /// Method for Info level
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        void Info(Exception ex, string message);
        /// <summary>
        /// Method for Info level
        /// </summary>
        /// <param name="message"></param>
        void Info(string message);

        /// <summary>
        /// Logging level enum
        /// </summary>
        LoggingLevelEnum? SelectedLevel { get; }
        /// <summary>
        /// Is Debug level are enabled
        /// </summary>
        bool IsDebugEnabled { get; }
        /// <summary>
        /// Is Error level are enabled
        /// </summary>
        bool IsErrorEnabled { get; }
        /// <summary>
        /// Is Fatal level are enabled
        /// </summary>
        bool IsFatalEnabled { get; }
        /// <summary>
        /// Is Info level are enabled
        /// </summary>
        bool IsInfoEnabled { get; }
        /// <summary>
        /// Is Trace level are enabled
        /// </summary>
        bool IsTraceEnabled { get; }
        /// <summary>
        /// Is Warn level are enabled
        /// </summary>
        bool IsWarnEnabled { get; }
    }
}
