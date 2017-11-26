using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
            services.AddWebEncoders();
            services.AddDataProtection();
            services.AddAuthentication(config =>{
                config.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = "oidc";
            }).AddOpenIdConnect("oidc", options =>{
                options.SignInScheme = "Cookies";
                options.Authority = "https://demo.identityserver.io/";
                options.RequireHttpsMetadata = false;
                options.ClientId = "implicit";
                options.SaveTokens = true;
            });
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            loggerFactory.AddSerilog();
            app.UseMiddleware<SerilogMiddleware>();
            app.UseAuthentication();

            //This is hacky :(
            app.Map("/account/logout", cfg => cfg.Run(async ctx => {
                await ctx.SignOutAsync("Cookies");
                await ctx.SignOutAsync("oidc", new AuthenticationProperties { RedirectUri = $"{ctx.Request.Scheme}://{ctx.Request.Host.Value}" });
              
            }));
            var settings = new AppSettings();
            ConfigurationBinder.Bind(configuration,settings);
            app.UseOwin(o => o.UseNancy(i => i.Bootstrapper = new CustomBootstrapper(settings)));

        }
    }
}