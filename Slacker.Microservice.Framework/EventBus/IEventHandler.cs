namespace Slacker.Microservice.Framework.EventBus
{
    public interface IEventHandler
    {
        void HandleEvent<T>(T @event) where T : Event;
    }
}