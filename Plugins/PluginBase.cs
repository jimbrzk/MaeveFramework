using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using MaeveFramework.Diagnostics;

namespace MaeveFramework.Plugins
{
    public abstract class PluginBase : IDisposable
    {
        protected readonly Logger Logger;

        public readonly string Name;
        public readonly string Version;
        public readonly string Author;
        public readonly string Description;

        public PluginBase()
        {
            var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetCallingAssembly().Location);
            Name = fvi.ProductName;
            Version = fvi.ProductVersion;
            Author = fvi.CompanyName;
            Description = fvi.FileDescription;

            Logger = new Logger($"MaeveFramework.Plugin.{Name}");
        }

        public virtual void Sleep(int ms)
        {
            Logger.Debug($"Sleep for: {TimeSpan.FromMilliseconds(ms).ToString("g")}");
            Thread.Sleep(ms);
        }

        public abstract void TikTok();

        public virtual void Dispose()
        {
        }
    }
}
