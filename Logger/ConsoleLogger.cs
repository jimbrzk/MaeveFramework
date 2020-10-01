using System;
using System.Collections.Generic;
using System.Text;

namespace MaeveFramework.Logger
{
    public class ConsoleLogger : ILogger
    {
        public void Debug(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        public void Debug(Exception ex, string message)
        {
            Console.WriteLine(message + Environment.NewLine + ex.ToString());
        }

        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        public void Error(Exception ex, string message)
        {
            Console.WriteLine(message + Environment.NewLine + ex.ToString());
        }

        public void Error(string message)
        {
            Console.WriteLine(message);
        }

        public void Info(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        public void Info(Exception ex, string message)
        {
            Console.WriteLine(message + Environment.NewLine + ex.ToString());
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Trace(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        public void Trace(Exception ex, string message)
        {
            Console.WriteLine(message + Environment.NewLine + ex.ToString());
        }

        public void Trace(string message)
        {
            Console.WriteLine(message);
        }

        public void Warn(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        public void Warn(Exception ex, string message)
        {
            Console.WriteLine(message + Environment.NewLine + ex.ToString());
        }

        public void Warn(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class ConsoleLogger<T> : ILogger<T>
    {
        public void Debug(Exception ex)
        {
            Console.WriteLine(nameof(T) + "| " + ex.ToString());
        }

        public void Debug(Exception ex, string message)
        {
            Console.WriteLine(message + Environment.NewLine + ex.ToString());
        }

        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        public void Error(Exception ex, string message)
        {
            Console.WriteLine(message + Environment.NewLine + ex.ToString());
        }

        public void Error(string message)
        {
            Console.WriteLine(message);
        }

        public void Info(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        public void Info(Exception ex, string message)
        {
            Console.WriteLine(message + Environment.NewLine + ex.ToString());
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Trace(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        public void Trace(Exception ex, string message)
        {
            Console.WriteLine(message + Environment.NewLine + ex.ToString());
        }

        public void Trace(string message)
        {
            Console.WriteLine(message);
        }

        public void Warn(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        public void Warn(Exception ex, string message)
        {
            Console.WriteLine(message + Environment.NewLine + ex.ToString());
        }

        public void Warn(string message)
        {
            Console.WriteLine(message);
        }
    }
}
