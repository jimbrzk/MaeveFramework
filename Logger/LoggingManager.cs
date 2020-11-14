using MaeveFramework.Logger.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MaeveFramework.Logger
{
    public class LoggingManager
    {
        private static ConstructorInfo SelectedLogger;

        public static MaeveFramework.Logger.Abstractions.ILogger GetLogger(string loggerName)
        {
            if (SelectedLogger == null) return typeof(NullLogger).GetConstructor(new[] { typeof(string) }).Invoke(new object[] { null }) as ILogger;
            return SelectedLogger.Invoke(new object[] { loggerName }) as ILogger;
        } 

        public static MaeveFramework.Logger.Abstractions.ILogger GetLogger()
        {
            if (SelectedLogger == null) return typeof(NullLogger).GetConstructor(new[] { typeof(string) }).Invoke(new object[] { null }) as ILogger;
            return SelectedLogger.Invoke(new object[] { Helpers.AbstrationsHelpers.NameOfCallingClass() }) as ILogger;
        }

        public static void UseNLog() 
        {
            Type type = typeof(NLogLogger);
            SelectedLogger = type.GetConstructor(new[] { typeof(string) });
        }

        public static void UseConsole()
        {
            Type type = typeof(ConsoleLogger);
            SelectedLogger = type.GetConstructor(new[] { typeof(string) });
        }

        public static void UseDebug()
        {
            Type type = typeof(DebugLogger);
            SelectedLogger = type.GetConstructor(new[] { typeof(string) });
        }
    }
}
