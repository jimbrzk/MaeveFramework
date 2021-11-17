using MaeveFramework.Logger.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MaeveFramework.Logger
{
    /// <summary>
    /// NLog Logger
    /// </summary>
    public class NLogLogger : LogLevel, ILogger
    {
        /// <summary>
        /// Log manager
        /// </summary>
        private readonly object _logManagerType;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="loggerName"></param>
        /// <param name="logLevel"></param>
        /// <exception cref="Exception"></exception>
        public NLogLogger(string loggerName, LoggingLevelEnum? logLevel = null)
        {
            var nLog = Type.GetType("NLog.LogManager,NLog");
            if (nLog == null) throw new Exception("Can't find NLog type assembly");
            _logManagerType = nLog.InvokeMember("GetLogger", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new[] { loggerName });
        }

        /// <inheritdoc cref="ILogger.Debug(Exception)" />
        public void Debug(Exception ex)
        {
            if (!IsDebugEnabled) return;
            _logManagerType.GetType().GetMethod("Debug", new Type[] { typeof(Exception) }).Invoke(_logManagerType, new object[] { ex });
        }

        /// <inheritdoc cref="ILogger.Debug(Exception, string)" />
        public void Debug(Exception ex, string message)
        {
            if (!IsDebugEnabled) return;
            _logManagerType.GetType().GetMethod("Debug", new Type[] { typeof(Exception), typeof(string) }).Invoke(_logManagerType, new object[] { ex, message });
        }

        /// <inheritdoc cref="ILogger.Debug(string)" />
        public void Debug(string message)
        {
            if (!IsDebugEnabled) return;
            _logManagerType.GetType().GetMethod("Debug", new Type[] { typeof(string) }).Invoke(_logManagerType, new object[] { message });
        }

        /// <inheritdoc cref="ILogger.Error(Exception)" />
        public void Error(Exception ex)
        {
            if (!IsErrorEnabled) return;
            _logManagerType.GetType().GetMethod("Error", new Type[] { typeof(Exception) }).Invoke(_logManagerType, new object[] { ex });
        }

        /// <inheritdoc cref="ILogger.Error(Exception, string)" />
        public void Error(Exception ex, string message)
        {
            if (!IsErrorEnabled) return;
            _logManagerType.GetType().GetMethod("Error", new Type[] { typeof(Exception), typeof(string) }).Invoke(_logManagerType, new object[] { ex, message });
        }

        /// <inheritdoc cref="ILogger.Error(string)" />
        public void Error(string message)
        {
            if (!IsErrorEnabled) return;
            _logManagerType.GetType().GetMethod("Error", new Type[] { typeof(string) }).Invoke(_logManagerType, new object[] { message });
        }

        /// <inheritdoc cref="ILogger.Info(Exception)" />
        public void Info(Exception ex)
        {
            if (!IsInfoEnabled) return;
            _logManagerType.GetType().GetMethod("Info", new Type[] { typeof(Exception) }).Invoke(_logManagerType, new object[] { ex });
        }

        /// <inheritdoc cref="ILogger.Info(Exception, string)" />
        public void Info(Exception ex, string message)
        {
            if (!IsInfoEnabled) return;
            _logManagerType.GetType().GetMethod("Info", new Type[] { typeof(Exception), typeof(string) }).Invoke(_logManagerType, new object[] { ex, message });
        }

        /// <inheritdoc cref="ILogger.Info(string)" />
        public void Info(string message)
        {
            if (!IsInfoEnabled) return;
            _logManagerType.GetType().GetMethod("Info", new Type[] { typeof(string) }).Invoke(_logManagerType, new object[] { message });
        }

        /// <inheritdoc cref="ILogger.Trace(Exception)" />
        public void Trace(Exception ex)
        {
            if (!IsTraceEnabled) return;
            _logManagerType.GetType().GetMethod("Trace", new Type[] { typeof(Exception) }).Invoke(_logManagerType, new object[] { ex });
        }

        /// <inheritdoc cref="ILogger.Trace(Exception, string)" />
        public void Trace(Exception ex, string message)
        {
            if (!IsTraceEnabled) return;
            _logManagerType.GetType().GetMethod("Trace", new Type[] { typeof(Exception), typeof(string) }).Invoke(_logManagerType, new object[] { ex, message });
        }

        /// <inheritdoc cref="ILogger.Trace(string)" />
        public void Trace(string message)
        {
            if (!IsTraceEnabled) return;
            _logManagerType.GetType().GetMethod("Trace", new Type[] { typeof(string) }).Invoke(_logManagerType, new object[] { message });
        }

        /// <inheritdoc cref="ILogger.Warn(Exception)" />
        public void Warn(Exception ex)
        {
            if (!IsWarnEnabled) return;
            _logManagerType.GetType().GetMethod("Warn", new Type[] { typeof(Exception) }).Invoke(_logManagerType, new object[] { ex });
        }

        /// <inheritdoc cref="ILogger.Warn(Exception, string)" />
        public void Warn(Exception ex, string message)
        {
            if (!IsWarnEnabled) return;
            _logManagerType.GetType().GetMethod("Warn", new Type[] { typeof(Exception), typeof(string) }).Invoke(_logManagerType, new object[] { ex, message });
        }

        /// <inheritdoc cref="ILogger.Warn(string)" />
        public void Warn(string message)
        {
            if (!IsWarnEnabled) return;
            _logManagerType.GetType().GetMethod("Warn", new Type[] { typeof(string) }).Invoke(_logManagerType, new object[] { message });
        }
    }
}
