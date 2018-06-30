using Slacker.Microservice.Framework.EventBus;

namespace Slacker.Microserver.Sample.Events
{
    public class WorkItemAdded : Event
    {
        public WorkItemAdded(string subject)
        {
            Subject = subject;
        }

        public string Subject { get; }
    }
}