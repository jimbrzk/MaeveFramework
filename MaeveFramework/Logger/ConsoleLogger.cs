using MaeveFramework.Logger.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaeveFramework.Logger
{
    /// <summary>
    /// Console logger
    /// </summary>
    public class ConsoleLogger : LogLevel, ILogger
    {
        /// <summary>
        /// Logger name
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="loggerName"></param>
        /// <param name="logLevel"></param>
        public ConsoleLogger(string loggerName, LoggingLevelEnum? logLevel = null) : base(logLevel)
        {
            Name = loggerName;
        }

        /// <inheritdoc cref="ILogger.Debug(Exception)" />
        public void Debug(Exception ex)
        {
            if (!IsDebugEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Debug| " + ex.ToString());
        }

        /// <inheritdoc cref="ILogger.Debug(Exception, string)" />
        public void Debug(Exception ex, string message)
        {
            if (!IsDebugEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Debug| " + message + Environment.NewLine + ex.ToString());
        }

        /// <inheritdoc cref="ILogger.Debug(string)" />
        public void Debug(string message)
        {
            if (!IsDebugEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Debug| " + message);
        }

        /// <inheritdoc cref="ILogger.Error(Exception)" />
        public void Error(Exception ex)
        {
            if (!IsErrorEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Error| " + ex.ToString());
        }

        /// <inheritdoc cref="ILogger.Error(Exception, string)" />
        public void Error(Exception ex, string message)
        {
            if (!IsErrorEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Error| " + message + Environment.NewLine + ex.ToString());
        }

        /// <inheritdoc cref="ILogger.Error(string)" />
        public void Error(string message)
        {
            if (!IsErrorEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Error| " + message);
        }

        /// <inheritdoc cref="ILogger.Info(Exception)" />
        public void Info(Exception ex)
        {
            if (!IsInfoEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Info| " + ex.ToString());
        }

        /// <inheritdoc cref="ILogger.Info(Exception, string)" />
        public void Info(Exception ex, string message)
        {
            if (!IsInfoEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Info| " + message + Environment.NewLine + ex.ToString());
        }

        /// <inheritdoc cref="ILogger.Info(string)" />
        public void Info(string message)
        {
            if (!IsInfoEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Info| " + message);
        }

        /// <inheritdoc cref="ILogger.Trace(Exception)" />
        public void Trace(Exception ex)
        {
            if (!IsTraceEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Trace| " + ex.ToString());
        }

        /// <inheritdoc cref="ILogger.Trace(Exception, string)" />
        public void Trace(Exception ex, string message)
        {
            if (!IsTraceEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Trace| " + message + Environment.NewLine + ex.ToString());
        }

        /// <inheritdoc cref="ILogger.Trace(string)" />
        public void Trace(string message)
        {
            if (!IsTraceEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Trace| " + message);
        }

        /// <inheritdoc cref="ILogger.Warn(Exception)" />
        public void Warn(Exception ex)
        {
            if (!IsWarnEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Trace| " + ex.ToString());
        }

        /// <inheritdoc cref="ILogger.Warn(Exception, string)" />
        public void Warn(Exception ex, string message)
        {
            if (!IsWarnEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Trace| " + message + Environment.NewLine + ex.ToString());
        }

        /// <inheritdoc cref="ILogger.Warn(string)" />
        public void Warn(string message)
        {
            if (!IsWarnEnabled) return;
            Console.WriteLine($"{DateTime.Now} |{Name}|Trace| " + message);
        }
    }
}
