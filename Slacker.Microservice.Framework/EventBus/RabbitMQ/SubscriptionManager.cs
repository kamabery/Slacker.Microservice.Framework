using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Slacker.Microservice.Framework.EventBus.RabbitMQ
{
    public interface ISubscriptionManager
    {
        void AddEvent(Type type);
        Task HandleMessage(string name, string message);
    }

    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly IServiceProvider provider;
        private readonly ILogger<SubscriptionManager> logger;
        private readonly List<Type> eventTypes;

        public SubscriptionManager(IServiceProvider provider, ILogger<SubscriptionManager> logger)
        {
            this.provider = provider;
            this.logger = logger;
            this.eventTypes = new List<Type>();
        }

        public void AddEvent(Type type)
        {
            // Only register type once
            if (!eventTypes.Contains(type))
            {
                this.eventTypes.Add(type);
            }
            
        }

        public async Task HandleMessage(string name, string message)
        {
            var type = this.eventTypes.FirstOrDefault((e)=> e.Name == name);
            if (type == null)
            {
                logger.LogCritical($"Event Handler was not setup {name} ");
                return;
            }
            try
            {
                var concreteType = typeof(IEventHandler<>).MakeGenericType(type);
                var @event = JsonConvert.DeserializeObject(message, type);
                var service = provider.GetService(concreteType);
                await (Task) concreteType.GetMethod("HandleEvent")?.Invoke(service, new[] {@event});
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Faild to Find Event Handler for" + name);
                return;
            }
            
            this.logger.LogInformation("Event Handled " + name);
        }


    }
}