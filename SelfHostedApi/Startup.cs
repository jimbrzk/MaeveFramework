using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MaeveFramework.SelfHostedApi
{
    public class Startup
    {
        public ServerJobOptions JobOptions => MaeveFramework.Scheduler.Manager.GetJobOptions<ServerJobOptions>(nameof(ServerJob));
        public ILogger Logger => MaeveFramework.Scheduler.Manager.GetJobLogger<ServerJob>();

        public Startup()
        {
            Logger.Debug("Starting selfhostedapi");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Scan assemblies for controllers
            List<Assembly> controllersAssemblies = new List<Assembly>();
            try
            {
                Assembly asm = Assembly.GetEntryAssembly();
                controllersAssemblies.AddRange(asm.GetTypes()
                    .Where(type => typeof(ControllerBase).IsAssignableFrom(type))
                    .Select(a => a.Assembly));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error on getting controllers assemblies");
            }
            finally
            {
                Logger.Debug($"Founded {controllersAssemblies.Count} assemblies with controllers");
            }

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            services.AddMvc(mvc =>
            {
                mvc.EnableEndpointRouting = false;
            });

            // Register controller from other assembiles
            foreach (var assembly in controllersAssemblies.Where(x => x != null))
            {
                services.AddMvc().PartManager.ApplicationParts.Add(new AssemblyPart(assembly));
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors("CorsPolicy");
            app.UseMvc();
        }
    }
}
