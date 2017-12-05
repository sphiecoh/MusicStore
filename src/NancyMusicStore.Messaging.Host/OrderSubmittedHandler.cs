namespace NancyMusicStore.Messaging.Host
{
    using System.Threading.Tasks;
    using NServiceBus;
    using NServiceBus.Logging;

    public class OrderSubmittedHandler : IHandleMessages<OrderSubmitted>
    {
        static ILog log = LogManager.GetLogger<OrderSubmittedHandler>();
        public Task Handle(OrderSubmitted message, IMessageHandlerContext context)
        {
             log.Info($"Processing order {message.OrderId} for {message.Username}");
            return Task.CompletedTask;
        }
    }
}