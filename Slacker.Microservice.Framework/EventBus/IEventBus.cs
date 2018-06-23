namespace Slacker.Microservice.Framework.EventBus
{
    public interface IEventBus
    {
        void Publish(Event @event);

        void Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler;

    }
}