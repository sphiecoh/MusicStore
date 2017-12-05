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
using NancyMusicStore.Messaging;
using StructureMap;

namespace NancyMusicStore
{
    public class CustomBootstrapper : Nancy.Bootstrappers.StructureMap.StructureMapNancyBootstrapper
    {
       
        private readonly IContainer container;
        public CustomBootstrapper(IContainer container)
        {
            this.container = container;
           
        }

        protected override void ApplicationStartup(IContainer container, IPipelines pipelines)
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
        protected override IContainer GetApplicationContainer() => container;
        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Scripts"));
            conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Content"));
        }




    }
}