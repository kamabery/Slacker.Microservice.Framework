using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Slacker.Microserver.Sample.Events;
using Slacker.Microservice.Framework.EventBus;

namespace Slacker.Microserver.Sample.EventHandlers
{
    public class WorkItemAddedEventHandler : IEventHandler<WorkItemAdded>
    {
        private readonly ILogger<WorkItemAddedEventHandler> logger;
        private readonly IEventBus bus;

        public WorkItemAddedEventHandler(ILogger<WorkItemAddedEventHandler> logger, IEventBus bus)
        {
            this.logger = logger;
            this.bus = bus;
        }

        public Task HandleEvent(WorkItemAdded @event) 
        {
            this.logger.LogInformation(@event.Subject);
            bus.Publish(new WorkItemLogged(@event.Subject, DateTime.Now));
            return Task.FromResult(0);
        }
    }
}