using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NancyMusicStore.Messaging;
using NServiceBus;

namespace NancyMusicStore
{
    public static class NServicebusBuilder
    {
        public static void AddServiceBus(this IServiceCollection services)
        {
            var endpointConfiguration = new EndpointConfiguration("Samples.ASPNETCore.Sender");
            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            endpointConfiguration.UsePersistence<LearningPersistence>();
            endpointConfiguration.SendOnly();
            var routing = transport.Routing();
            //routing.RouteToEndpoint(
            //assembly: typeof(OrderSubmitted).Assembly,
            //destination: "Samples.ASPNETCore.Endpoint");
            var endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
            services.AddSingleton<IMessageSession>(endpointInstance);

        }
    }
}