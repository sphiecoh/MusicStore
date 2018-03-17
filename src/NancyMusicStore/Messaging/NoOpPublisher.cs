using System;

namespace NancyMusicStore.Messaging
{
    public class NoOpPublisher : IBasicPublisher
    {
        public void SendMessage(object message, string correlation = null)
        {
           
        }
    }
}