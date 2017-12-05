using System;
using NServiceBus;
using System.Threading.Tasks;

namespace NancyMusicStore.Messaging.Host
{
    class Program
    {
        static async Task Main(string[] args)
        {
           Console.Title = "Samples.ASPNETCore.Endpoint";
            var endpointConfiguration = new EndpointConfiguration("Samples.ASPNETCore.Endpoint");
            endpointConfiguration.UsePersistence<LearningPersistence>();
            endpointConfiguration.UseTransport<LearningTransport>();

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            await endpointInstance.Stop()
            .ConfigureAwait(false);
        }
    }
}
