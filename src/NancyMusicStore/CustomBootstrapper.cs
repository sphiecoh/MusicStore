using Nancy;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.Conventions;
using Nancy.Session;
using Nancy.TinyIoc;
using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using NancyMusicStore.Common;
using NancyMusicStore.Models;

namespace NancyMusicStore
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        private IConfiguration configuration;
        public CustomBootstrapper(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override void ApplicationStartup(TinyIoCContainer container,IPipelines pipelines)
        {
            //enable the cookie
            CookieBasedSessions.Enable(pipelines);
            //Prevent errors on Linux

        }

        public override void Configure(INancyEnvironment environment)
        {
            base.Configure(environment);
            environment.Tracing(false, true);
            environment.Views(false, true);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register(new HttpClient { BaseAddress = new Uri(configuration["shippingApi"]) });
            container.Register<IDbHelper>((y,_) => new DBHelper(configuration.GetConnectionString("pgsqlConn")));
            container.Register(typeof(ShoppingCart));
        }

        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Scripts"));
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Content"));
        }

        

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);
            
        }
    }
}