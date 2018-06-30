using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Slacker.Microserver.Sample.Events;
using Slacker.Microserver.Sample.Models;
using Slacker.Microservice.Framework.EventBus;

namespace Slacker.Microserver.Sample.Controllers
{
    [Route("[controller]")]
    public class WorkItemController : Controller
    {
        private readonly ILogger<WorkItemController> logger;
        private readonly IEventBus bus;

        public WorkItemController(ILogger<WorkItemController> logger, IEventBus bus)
        {
            this.logger = logger;
            this.bus = bus;
        }

        [HttpPost]
        public void AddWorkItem(WorkItem workItem)
        {
            this.logger.LogInformation($"Work Item added through Controller ");
            this.bus.Publish(new WorkItemAdded(workItem.Subject));
        }
    }
}