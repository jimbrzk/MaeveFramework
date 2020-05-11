using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MaeveFramework.Diagnostics
{
    public class Logger : ILogger
    {
        private readonly object _logger;

        public Logger(string loggerName)
        {
            try
            {
                var nLog = Type.GetType("MaeveFramework.LogManager,MaeveFramework");
                _logger = nLog.InvokeMember("CreateLogger",
                    BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null,
                    new[] {loggerName});
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to initialize logger", ex);
            }
        }

        public void Error(Exception ex, string message)
        {
            if(_logger == null)
                return;

            _logger.GetType().GetMethod("Error").Invoke(_logger, new object[] {ex, message});
        }

        public void Debug(string message)
        {
            if (_logger == null)
                return;

            _logger.GetType().GetMethod("Debug").Invoke(_logger, new object[] { message });
        }

        public void Warn(string message)
        {
            if (_logger == null)
                return;

            _logger.GetType().GetMethod("Warn").Invoke(_logger, new object[] { message });
        }

        public void Info(string message)
        {
            if (_logger == null)
                return;

            _logger.GetType().GetMethod("Info").Invoke(_logger, new object[] { message });
        }
    }
}
