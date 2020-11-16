using MaeveFramework.Logger.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaeveFramework.Logger
{
    public class ConsoleLogger : LogLevel, ILogger
    {
        public readonly string Name;

        public ConsoleLogger(string loggerName, LoggingLevelEnum? logLevel = null) : base(logLevel)
        {
            Name = loggerName;
        }

        public void Debug(Exception ex)
        {
            if (!IsDebugEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Debug| " + ex.ToString());
        }

        public void Debug(Exception ex, string message)
        {
            if (!IsDebugEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Debug| " + message + Environment.NewLine + ex.ToString());
        }

        public void Debug(string message)
        {
            if (!IsDebugEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Debug| " + message);
        }

        public void Error(Exception ex)
        {
            if (!IsErrorEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Error| " + ex.ToString());
        }

        public void Error(Exception ex, string message)
        {
            if (!IsErrorEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Error| " + message + Environment.NewLine + ex.ToString());
        }

        public void Error(string message)
        {
            if (!IsErrorEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Error| " + message);
        }

        public void Info(Exception ex)
        {
            if (!IsInfoEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Info| " + ex.ToString());
        }

        public void Info(Exception ex, string message)
        {
            if (!IsInfoEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Info| " + message + Environment.NewLine + ex.ToString());
        }

        public void Info(string message)
        {
            if (!IsInfoEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Info| " + message);
        }

        public void Trace(Exception ex)
        {
            if (!IsTraceEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Trace| " + ex.ToString());
        }

        public void Trace(Exception ex, string message)
        {
            if (!IsTraceEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Trace| " + message + Environment.NewLine + ex.ToString());
        }

        public void Trace(string message)
        {
            if (!IsTraceEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Trace| " + message);
        }

        public void Warn(Exception ex)
        {
            if (!IsWarnEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Trace| " + ex.ToString());
        }

        public void Warn(Exception ex, string message)
        {
            if (!IsWarnEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Trace| " + message + Environment.NewLine + ex.ToString());
        }

        public void Warn(string message)
        {
            if (!IsWarnEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Trace| " + message);
        }
    }
}
