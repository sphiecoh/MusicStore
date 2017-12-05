using System;
using NServiceBus;
namespace NancyMusicStore.Messaging
{

    public class OrderSubmitted : IEvent
    {
         public int OrderId { get; set; }
         public string Username { get; set; }
    }
}
