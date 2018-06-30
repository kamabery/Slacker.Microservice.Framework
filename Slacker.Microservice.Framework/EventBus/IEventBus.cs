using System.Threading.Tasks;

namespace Slacker.Microservice.Framework.EventBus
{
    public interface IEventBus
    {
        bool Publish(Event @event);

        bool Subscribe<T>()
            where T : Event;
    }
}