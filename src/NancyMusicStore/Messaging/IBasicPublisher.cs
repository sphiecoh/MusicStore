namespace NancyMusicStore.Messaging
{
    public interface IBasicPublisher
    {
        void SendMessage(object message, string correlation = null);
    }
}