using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MaeveFramework.Scheduler;
using MaeveFramework.Scheduler.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MaeveFramework.SelfHostedApi
{
    public class ServerJobOptions
    {
        public string RootDir { get; set; }
        public string BaseAddress { get; set; }
        public long ConcurrentConnections { get; set; }
    }

    public class ServerJob : JobBase
    {
        private IWebHost _webApp;
        public ServerJobOptions Options => base.GetJobOptions<ServerJobOptions>();

        public ServerJob(Schedule schedule, ServerJobOptions options) : base(schedule, options)
        {

        }

        public override void OnStart()
        {
            base.OnStart();

            var config = new ConfigurationBuilder()
                .Build();

            if (!Directory.Exists(Options.RootDir))
            {
                Logger.Debug($"root directory in {Options.RootDir} not existing, creating folder.");
                Directory.CreateDirectory(Options.RootDir);
            }

            _webApp = new WebHostBuilder()
                .UseConfiguration(config)
                .ConfigureLogging(log =>
                {
                    log.ClearProviders();
                })
                .UseUrls(Options.BaseAddress)
                .UseKestrel(kestrel =>
                {
                    kestrel.AddServerHeader = false;
                    kestrel.Limits.MaxConcurrentConnections = Options.ConcurrentConnections;
                    kestrel.Configure(config);
                })
                .UseContentRoot(Options.RootDir)
                .UseStartup<Startup>()
                .Build();

            Logger.Info($"Starting Kestrel at {Options.BaseAddress} with {Options.ConcurrentConnections} councurrent connections limit");

            _webApp.Start();
        }

        public override void Job()
        {
            base.Job();

            Logger.Debug("Kestrel is running");
            _webApp.WaitForShutdown();
        }

        public override void OnStop()
        {
            base.OnStop();

            _webApp?.StopAsync(TimeSpan.FromSeconds(3))?.Wait(4000);
            _webApp?.Dispose();
        }
    }
}
