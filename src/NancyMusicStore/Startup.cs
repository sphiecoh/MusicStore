using System.IdentityModel.Tokens.Jwt;
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
        public Startup(IHostingEnvironment env)
        {
            Log.Logger = new LoggerConfiguration()
           .WriteTo.RollingFile("logs/log-{Date}.log")
           .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
           .MinimumLevel.Override("System", LogEventLevel.Warning).CreateLogger();
             
             
             configuration = new ConfigurationBuilder()
             .SetBasePath(env.ContentRootPath)
            .AddEnvironmentVariables(prefix: "ASPNETCORE_")
            .AddJsonFile("appsettings.json",true)
            .Build();

        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(configuration);
            services.AddWebEncoders();
            services.AddDataProtection();
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            loggerFactory.AddSerilog();
            app.UseMiddleware<SerilogMiddleware>();
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies"
            });



            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                AuthenticationScheme = "oidc",
                SignInScheme = "Cookies",
                Authority = "https://demo.identityserver.io/",
                RequireHttpsMetadata = false,
                ClientId = "implicit",
                SaveTokens = true,
               

            });

            //This is hacky :(
            app.Map("/account/logout", cfg => cfg.Run(async ctx => {
                await ctx.Authentication.SignOutAsync("Cookies");
                await ctx.Authentication.SignOutAsync("oidc", new Microsoft.AspNetCore.Http.Authentication.AuthenticationProperties { RedirectUri = $"{ctx.Request.Scheme}://{ctx.Request.Host.Value}" });
              
            }));

            app.UseOwin(o => o.UseNancy(i => i.Bootstrapper = new CustomBootstrapper(configuration)));

        }
    }
}