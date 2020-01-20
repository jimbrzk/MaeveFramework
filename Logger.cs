using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MaeveFramework
{
    public class LogManager
    {
        private static bool _console;
        private static bool _nlog;

        public static void UseConsole()
        {
            _console = true;
        }

        public static void UseNLog()
        {
            _nlog = true;
        }

        public static ILogger CreateLogger(string loggerName = "MaeveFramework")
        {
            object logManagerType = null;

            if (_nlog)
            {
                try
                {
                    var nLog = Type.GetType("NLog.LogManager,NLog");
                    logManagerType = nLog.InvokeMember("GetLogger", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new[] { loggerName });
                }
                catch (NullReferenceException ex)
                {
                    throw new Exception("NLog not loaded", ex);
                }
                catch (MissingMemberException ex)
                {
                    throw new Exception("Incompatibility NLog version are loaded", ex);
                }
            }

            return new Logger(loggerName, _console, logManagerType);
        }

        protected class Logger : ILogger
        {
            private readonly bool _console;
            private readonly object _nlogLogger;
            private readonly MethodInfo _errorNLogLogger;
            private readonly MethodInfo _debugNLogLogger;
            private readonly MethodInfo _infoNLogLogger;
            private readonly MethodInfo _warnNLogLogger;
            public readonly string LoggerName;

            public Logger(string loggerName, bool console = true, object nlogLogger = null)
            {
                LoggerName = loggerName;
                _console = console;
                _nlogLogger = nlogLogger;
                if (_nlogLogger != null)
                {
                    _errorNLogLogger = _nlogLogger.GetType().GetMethods().Where(x => x.Name == "Error").ToArray()[36];
                    _debugNLogLogger = _nlogLogger.GetType().GetMethods().Where(x => x.Name == "Debug").ToArray()[34];
                    _infoNLogLogger = _nlogLogger.GetType().GetMethods().Where(x => x.Name == "Info").ToArray()[34];
                    _warnNLogLogger = _nlogLogger.GetType().GetMethods().Where(x => x.Name == "Warn").ToArray()[34];
                }
            }

            public void Error(Exception ex, string message)
            {
                if (_console)
                    Console.WriteLine($"[Error][{DateTime.Now.ToString("G")}][{LoggerName}] {message}" + ((ex != null) ? Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace : string.Empty));
                if (_nlogLogger != null)
                    _errorNLogLogger.Invoke(_nlogLogger, new object[] { ex, message });
            }

            public void Info(string message)
            {
                if (_console)
                    Console.WriteLine($"[Info][{DateTime.Now.ToString("G")}][{LoggerName}] {message}");
                if (_nlogLogger != null)
                    _infoNLogLogger.Invoke(_nlogLogger, new object[] { message });
            }

            public void Debug(string message)
            {
                if (_console)
                    Console.WriteLine($"[Debug][{DateTime.Now.ToString("G")}][{LoggerName}] {message}");
                if (_nlogLogger != null)
                    _debugNLogLogger.Invoke(_nlogLogger, new object[] { message });
            }

            public void Warn(string message)
            {
                if (_console)
                    Console.WriteLine($"[Warn][{DateTime.Now.ToString("G")}][{LoggerName}] {message}");
                if (_nlogLogger != null)
                    _warnNLogLogger.Invoke(_nlogLogger, new object[] { message });
            }
        }
    }

    public interface ILogger
    {
        void Error(Exception ex, string message);
        void Debug(string message);
        void Warn(string message);
        void Info(string message);
    }
}
