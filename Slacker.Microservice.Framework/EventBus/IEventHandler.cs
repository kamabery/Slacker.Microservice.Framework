using System.Threading.Tasks;

namespace Slacker.Microservice.Framework.EventBus
{
    public interface IEventHandler<T> where T : Event
    {
        Task HandleEvent(T @event);
    }
}