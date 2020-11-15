using MaeveFramework.Logger.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaeveFramework.Logger
{
    public class DebugLogger : ILogger
    {
        public readonly string Name;

        public DebugLogger(string loggerName)
        {
            Name = loggerName;
        }

        public void Debug(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"{Name}|Debug| " + ex.ToString());
        }

        public void Debug(Exception ex, string message)
        {
            System.Diagnostics.Debug.WriteLine($"{Name}|Debug| " + message + Environment.NewLine + ex.ToString());
        }

        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine($"{Name}|Debug| " + message);
        }

        public void Error(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"{Name}|Error| " + ex.ToString());
        }

        public void Error(Exception ex, string message)
        {
            System.Diagnostics.Debug.WriteLine($"{Name}|Error| " + message + Environment.NewLine + ex.ToString());
        }

        public void Error(string message)
        {
            System.Diagnostics.Debug.WriteLine($"{Name}|Error| " + message);
        }

        public void Info(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"{Name}|Info| " + ex.ToString());
        }

        public void Info(Exception ex, string message)
        {
            System.Diagnostics.Debug.WriteLine($"{Name}|Info| " + message + Environment.NewLine + ex.ToString());
        }

        public void Info(string message)
        {
            System.Diagnostics.Debug.WriteLine($"{Name}|Info| " + message);
        }

        public void Trace(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"{Name}|Trace| " + ex.ToString());
        }

        public void Trace(Exception ex, string message)
        {
            System.Diagnostics.Debug.WriteLine($"{Name}|Trace| " + message + Environment.NewLine + ex.ToString());
        }

        public void Trace(string message)
        {
            System.Diagnostics.Debug.WriteLine($"{Name}|Trace| " + message);
        }

        public void Warn(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"{Name}|Trace| " + ex.ToString());
        }

        public void Warn(Exception ex, string message)
        {
            System.Diagnostics.Debug.WriteLine($"{Name}|Trace| " + message + Environment.NewLine + ex.ToString());
        }

        public void Warn(string message)
        {
            System.Diagnostics.Debug.WriteLine($"{Name}|Trace| " + message);
        }
    }
}
