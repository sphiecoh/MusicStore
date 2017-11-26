using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nancy.Owin;
using Serilog;
using Serilog.Events;

namespace NancyMusicStore
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            Log.Logger = new LoggerConfiguration()
           .WriteTo.RollingFile("logs/log-{Date}.log")
           .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
           .MinimumLevel.Override("System", LogEventLevel.Warning).CreateLogger();
             
             
             configuration = config;

        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(configuration);
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();
            app.UseMiddleware<SerilogMiddleware>();
            app.UseOwin(o => o.UseNancy(i => i.Bootstrapper = new CustomBootstrapper(configuration)));

        }
    }
}