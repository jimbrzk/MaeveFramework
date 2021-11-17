using System;
using System.Collections.Generic;
using System.Text;

namespace MaeveFramework.Logger.Abstractions
{
    /// <summary>
    /// Logging level enum
    /// </summary>
    public enum LoggingLevelEnum
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Debug,
        Error,
        Fatal,
        Info,
        Off,
        Trace,
        Warn
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// LogLevel abstraction
    /// </summary>
    public abstract class LogLevel
    {
        /// <summary>
        /// Selected log level
        /// </summary>
        public LoggingLevelEnum? SelectedLevel { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="level"></param>
        public LogLevel(LoggingLevelEnum? level = null)
        {
            SelectedLevel = level;
        }

        /// <summary>
        /// Is Debug levle enabled
        /// </summary>
        public bool IsDebugEnabled
        {
            get
            {
                if (!SelectedLevel.HasValue) return true;
                if (SelectedLevel == (LoggingLevelEnum.Debug | LoggingLevelEnum.Error | LoggingLevelEnum.Fatal | LoggingLevelEnum.Info | LoggingLevelEnum.Warn)) return true;
                return false;
            }
        }

        /// <summary>
        /// Is Error level enabled
        /// </summary>
        public bool IsErrorEnabled
        {
            get
            {
                if (!SelectedLevel.HasValue) return true;
                if (SelectedLevel == (LoggingLevelEnum.Error | LoggingLevelEnum.Fatal | LoggingLevelEnum.Info | LoggingLevelEnum.Warn)) return true;
                return false;
            }
        }

        /// <summary>
        /// Is Fatal level enabled
        /// </summary>
        public bool IsFatalEnabled
        {
            get
            {
                if (!SelectedLevel.HasValue) return true;
                if (SelectedLevel == (LoggingLevelEnum.Error | LoggingLevelEnum.Fatal | LoggingLevelEnum.Info | LoggingLevelEnum.Warn)) return true;
                return false;
            }
        }

        /// <summary>
        /// Is info level enabled
        /// </summary>
        public bool IsInfoEnabled
        {
            get
            {
                if (!SelectedLevel.HasValue) return true;
                if (SelectedLevel == (LoggingLevelEnum.Error | LoggingLevelEnum.Fatal | LoggingLevelEnum.Info | LoggingLevelEnum.Warn)) return true;
                return false;
            }
        }

        /// <summary>
        /// Is Trace level enabled
        /// </summary>
        public bool IsTraceEnabled
        {
            get
            {
                if (!SelectedLevel.HasValue) return true;
                if (SelectedLevel == LoggingLevelEnum.Trace) return true;
                return false;
            }
        }

        /// <summary>
        /// Is Warn level enabled
        /// </summary>
        public bool IsWarnEnabled
        {
            get
            {
                if (!SelectedLevel.HasValue) return true;
                if (SelectedLevel == (LoggingLevelEnum.Error | LoggingLevelEnum.Fatal | LoggingLevelEnum.Info | LoggingLevelEnum.Warn)) return true;
                return false;
            }
        }
    }
}
