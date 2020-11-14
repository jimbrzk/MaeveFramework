using MaeveFramework.Logger.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaeveFramework.Logger
{
    public class ConsoleLogger : ILogger
    {
        public readonly string Name;

        public ConsoleLogger(string loggerName)
        {
            Name = loggerName;
        }

        public void Debug(Exception ex)
        {
            Console.WriteLine($"{Name}|Debug| " + ex.ToString());
        }

        public void Debug(Exception ex, string message)
        {
            Console.WriteLine($"{Name}|Debug| " + message + Environment.NewLine + ex.ToString());
        }

        public void Debug(string message)
        {
            Console.WriteLine($"{Name}|Debug| " + message);
        }

        public void Error(Exception ex)
        {
            Console.WriteLine($"{Name}|Error| " + ex.ToString());
        }

        public void Error(Exception ex, string message)
        {
            Console.WriteLine($"{Name}|Error| " + message + Environment.NewLine + ex.ToString());
        }

        public void Error(string message)
        {
            Console.WriteLine($"{Name}|Error| " + message);
        }

        public void Info(Exception ex)
        {
            Console.WriteLine($"{Name}|Info| " + ex.ToString());
        }

        public void Info(Exception ex, string message)
        {
            Console.WriteLine($"{Name}|Info| " + message + Environment.NewLine + ex.ToString());
        }

        public void Info(string message)
        {
            Console.WriteLine($"{Name}|Info| " + message);
        }

        public void Trace(Exception ex)
        {
            Console.WriteLine($"{Name}|Trace| " + ex.ToString());
        }

        public void Trace(Exception ex, string message)
        {
            Console.WriteLine($"{Name}|Trace| " + message + Environment.NewLine + ex.ToString());
        }

        public void Trace(string message)
        {
            Console.WriteLine($"{Name}|Trace| " + message);
        }

        public void Warn(Exception ex)
        {
            Console.WriteLine($"{Name}|Trace| " + ex.ToString());
        }

        public void Warn(Exception ex, string message)
        {
            Console.WriteLine($"{Name}|Trace| " + message + Environment.NewLine + ex.ToString());
        }

        public void Warn(string message)
        {
            Console.WriteLine($"{Name}|Trace| " + message);
        }
    }
}
