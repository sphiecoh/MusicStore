using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nancy.Owin;
using NancyMusicStore.Common;
using NancyMusicStore.Models;
using NServiceBus;
using Serilog;
using Serilog.Events;
using StructureMap;

namespace NancyMusicStore
{
    public class Startup
    {
        private IContainer _container;
        private readonly IConfiguration configuration;
        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            Log.Logger = new LoggerConfiguration()
           .WriteTo.RollingFile("logs/log-{Date}.log")
           .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
           .MinimumLevel.Override("System", LogEventLevel.Warning).CreateLogger();
             
             
             configuration = config;

        }
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddServiceBus();
            var settings = new AppSettings();
            configuration.Bind(settings);
            services.AddSingleton(configuration);
            services.AddWebEncoders();
            services.AddDataProtection();
            services.AddAuthentication(config =>{
                config.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = "oidc";
            })
            .AddCookie()
            .AddOpenIdConnect("oidc", options =>{
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = "https://demo.identityserver.io/";
                options.RequireHttpsMetadata = false;
                options.ClientId = "implicit";
                options.SaveTokens = true;
            });
            _container = new Container();
            _container.Configure(x => {
                x.For<IDbHelper>().Use(y => new DBHelper(settings.DatabaseConnection));
                x.ForConcreteType<ShoppingCart>().Configure.Singleton();
                x.Scan(y => {
                    y.WithDefaultConventions();
                    y.TheCallingAssembly();
                });
            });
            NancyMusicStore.Database.SeedData.Populate(settings.DatabaseConnection);
            _container.Populate(services);
        
            return _container.GetInstance<IServiceProvider>();
        }
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            loggerFactory.AddSerilog();
            loggerFactory.AddDebug();
            app.UseMiddleware<SerilogMiddleware>();
            app.UseAuthentication();

            //This is hacky :(
            app.Map("/account/logout", cfg => cfg.Run(async ctx => {
                await ctx.SignOutAsync("Cookies");
                await ctx.SignOutAsync("oidc", new AuthenticationProperties { RedirectUri = $"{ctx.Request.Scheme}://{ctx.Request.Host.Value}" });
              
            }));
           
            app.UseOwin(o => o.UseNancy(config => { 
                config.Bootstrapper = new CustomBootstrapper(_container);
                config.PassThroughWhenStatusCodesAre(Nancy.HttpStatusCode.Unauthorized);
            }));
            //Since upgrading to .net 2.0 => 401 response isn't redirecting to IdServer
            app.Use((cont,next) =>  cont.ChallengeAsync());
        }
       
    }
}