﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MaeveFramework.Logger.Abstractions
{
    /// <summary>
    /// Logging level enum
    /// </summary>
    public enum LoggingLevelEnum
    {
        Debug,
        Error,
        Fatal,
        Info,
        Off,
        Trace,
        Warn
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

        public bool IsDebugEnabled
        {
            get
            {
                if (!SelectedLevel.HasValue) return true;
                if (SelectedLevel == (LoggingLevelEnum.Debug | LoggingLevelEnum.Error | LoggingLevelEnum.Fatal | LoggingLevelEnum.Info | LoggingLevelEnum.Warn)) return true;
                return false;
            }
        }

        public bool IsErrorEnabled
        {
            get
            {
                if (!SelectedLevel.HasValue) return true;
                if (SelectedLevel == (LoggingLevelEnum.Error | LoggingLevelEnum.Fatal | LoggingLevelEnum.Info | LoggingLevelEnum.Warn)) return true;
                return false;
            }
        }

        public bool IsFatalEnabled
        {
            get
            {
                if (!SelectedLevel.HasValue) return true;
                if (SelectedLevel == (LoggingLevelEnum.Error | LoggingLevelEnum.Fatal | LoggingLevelEnum.Info | LoggingLevelEnum.Warn)) return true;
                return false;
            }
        }

        public bool IsInfoEnabled
        {
            get
            {
                if (!SelectedLevel.HasValue) return true;
                if (SelectedLevel == (LoggingLevelEnum.Error | LoggingLevelEnum.Fatal | LoggingLevelEnum.Info | LoggingLevelEnum.Warn)) return true;
                return false;
            }
        }

        public bool IsTraceEnabled
        {
            get
            {
                if (!SelectedLevel.HasValue) return true;
                if (SelectedLevel == LoggingLevelEnum.Trace) return true;
                return false;
            }
        }

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
