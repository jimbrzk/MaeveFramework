using MaeveFramework.Logger.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MaeveFramework.Logger
{
    /// <summary>
    /// Logging manager
    /// Use one of availble logger before use GetLogger
    /// <see cref="UseConsole"/>
    /// <see cref="UseDebug"/>
    /// <see cref="UseNLog"/>
    /// </summary>
    public class LoggingManager
    {
        private static ConstructorInfo SelectedLogger;

        /// <summary>
        /// Get logger with given logger name
        /// </summary>
        /// <param name="loggerName">Logger name</param>
        /// <param name="logLevel">Logging level, null will log all levels</param>
        /// <returns></returns>
        public static MaeveFramework.Logger.Abstractions.ILogger GetLogger(string loggerName, LoggingLevelEnum? logLevel = null)
        {
            if (SelectedLogger == null) return typeof(NullLogger).GetConstructor(new[] { typeof(string), typeof(LoggingLevelEnum?) }).Invoke(new object[] { null, logLevel }) as ILogger;
            return SelectedLogger.Invoke(new object[] { loggerName, logLevel }) as ILogger;
        }

        /// <summary>
        /// Get logger, calling class nam will be used as logger name
        /// </summary>
        /// <param name="logLevel">Logging level, null will log all levels</param>
        /// <returns></returns>
        public static MaeveFramework.Logger.Abstractions.ILogger GetLogger(LoggingLevelEnum? logLevel = null)
        {
            if (SelectedLogger == null) return typeof(NullLogger).GetConstructor(new[] { typeof(string), typeof(LoggingLevelEnum?) }).Invoke(new object[] { null, logLevel }) as ILogger;
            return SelectedLogger.Invoke(new object[] { Helpers.AbstrationsHelpers.NameOfCallingClass(), logLevel }) as ILogger;
        }

        /// <summary>
        /// Use NLog library for logging
        /// <see cref="GetLogger(LoggingLevelEnum?)"/>
        /// <seealso cref="GetLogger(string, LoggingLevelEnum?)"/>
        /// </summary>
        public static void UseNLog() 
        {
            Type type = typeof(NLogLogger);
            SelectedLogger = type.GetConstructor(new[] { typeof(string), typeof(LoggingLevelEnum?) });
        }

        /// <summary>
        /// Use Console.WriteLine for logging
        /// <see cref="GetLogger(LoggingLevelEnum?)"/>
        /// <seealso cref="GetLogger(string, LoggingLevelEnum?)"/>
        /// </summary>
        public static void UseConsole()
        {
            Type type = typeof(ConsoleLogger);
            SelectedLogger = type.GetConstructor(new[] { typeof(string), typeof(LoggingLevelEnum?) });
        }

        /// <summary>
        /// Use Debug.WriteLine for logging
        /// <see cref="GetLogger(LoggingLevelEnum?)"/>
        /// <seealso cref="GetLogger(string, LoggingLevelEnum?)"/>
        /// </summary>
        public static void UseDebug()
        {
            Type type = typeof(DebugLogger);
            SelectedLogger = type.GetConstructor(new[] { typeof(string), typeof(LoggingLevelEnum?) });
        }
    }
}
