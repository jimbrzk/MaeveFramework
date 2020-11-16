using MaeveFramework.Logger.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MaeveFramework.Logger
{
    public class NLogLogger : LogLevel, ILogger
    {
        private readonly object _logManagerType;

        public NLogLogger(string loggerName, LoggingLevelEnum? logLevel = null)
        {
            var nLog = Type.GetType("NLog.LogManager,NLog");
            if (nLog == null) throw new Exception("Can't find NLog type assembly");
            _logManagerType = nLog.InvokeMember("GetLogger", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new[] { loggerName });
        }

        public void Debug(Exception ex)
        {
            if (!IsDebugEnabled) return;
            _logManagerType.GetType().GetMethod("Debug", new Type[] { typeof(Exception) }).Invoke(_logManagerType, new object[] { ex });
        }

        public void Debug(Exception ex, string message)
        {
            if (!IsDebugEnabled) return;
            _logManagerType.GetType().GetMethod("Debug", new Type[] { typeof(Exception), typeof(string) }).Invoke(_logManagerType, new object[] { ex, message });
        }
        public void Debug(string message)
        {
            if (!IsDebugEnabled) return;
            _logManagerType.GetType().GetMethod("Debug", new Type[] { typeof(string) }).Invoke(_logManagerType, new object[] { message });
        }
        public void Error(Exception ex)
        {
            if (!IsErrorEnabled) return;
            _logManagerType.GetType().GetMethod("Error", new Type[] { typeof(Exception) }).Invoke(_logManagerType, new object[] { ex });
        }
        public void Error(Exception ex, string message)
        {
            if (!IsErrorEnabled) return;
            _logManagerType.GetType().GetMethod("Error", new Type[] { typeof(Exception), typeof(string) }).Invoke(_logManagerType, new object[] { ex, message });
        }
        public void Error(string message)
        {
            if (!IsErrorEnabled) return;
            _logManagerType.GetType().GetMethod("Error", new Type[] { typeof(string) }).Invoke(_logManagerType, new object[] { message });
        }
        public void Info(Exception ex)
        {
            if (!IsInfoEnabled) return;
            _logManagerType.GetType().GetMethod("Info", new Type[] { typeof(Exception) }).Invoke(_logManagerType, new object[] { ex });
        }
        public void Info(Exception ex, string message)
        {
            if (!IsInfoEnabled) return;
            _logManagerType.GetType().GetMethod("Info", new Type[] { typeof(Exception), typeof(string) }).Invoke(_logManagerType, new object[] { ex, message });
        }
        public void Info(string message)
        {
            if (!IsInfoEnabled) return;
            _logManagerType.GetType().GetMethod("Info", new Type[] { typeof(string) }).Invoke(_logManagerType, new object[] { message });
        }
        public void Trace(Exception ex)
        {
            if (!IsTraceEnabled) return;
            _logManagerType.GetType().GetMethod("Trace", new Type[] { typeof(Exception) }).Invoke(_logManagerType, new object[] { ex });
        }
        public void Trace(Exception ex, string message)
        {
            if (!IsTraceEnabled) return;
            _logManagerType.GetType().GetMethod("Trace", new Type[] { typeof(Exception), typeof(string) }).Invoke(_logManagerType, new object[] { ex, message });
        }
        public void Trace(string message)
        {
            if (!IsTraceEnabled) return;
            _logManagerType.GetType().GetMethod("Trace", new Type[] { typeof(string) }).Invoke(_logManagerType, new object[] { message });
        }
        public void Warn(Exception ex)
        {
            if (!IsWarnEnabled) return;
            _logManagerType.GetType().GetMethod("Warn", new Type[] { typeof(Exception) }).Invoke(_logManagerType, new object[] { ex });
        }
        public void Warn(Exception ex, string message)
        {
            if (!IsWarnEnabled) return;
            _logManagerType.GetType().GetMethod("Warn", new Type[] { typeof(Exception), typeof(string) }).Invoke(_logManagerType, new object[] { ex, message });
        }
        public void Warn(string message)
        {
            if (!IsWarnEnabled) return;
            _logManagerType.GetType().GetMethod("Warn", new Type[] { typeof(string) }).Invoke(_logManagerType, new object[] { message });
        }
    }
}
