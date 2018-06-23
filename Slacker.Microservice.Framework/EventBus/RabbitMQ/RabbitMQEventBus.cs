using System;
using Microsoft.Extensions.Logging;

namespace Slacker.Microservice.Framework.EventBus.RabbitMQ
{
    public class RabbitMQEventBus : IEventBus
    {
        private readonly RabbitMQOptions options;
        private readonly ILogger<RabbitMQEventBus> logger;

        public RabbitMQEventBus(RabbitMQOptions options, ILogger<RabbitMQEventBus> logger)
        {
            this.options = options;
            this.logger = logger;
        }

        public void Setup()
        {
            try
            {
                this.logger.LogInformation($"Connecting to Rabbit MQ Host: {options.HostName} User : {options.UserName}");
                if (options.Password.Length == 0)
                {
                    this.logger.LogInformation("Password is Empty");
                }

            }
            catch (Exception e)
            {
                this.logger.LogCritical(e.ToString());
                throw;
            }
        }

        public void Publish(Event @event)
        {
            throw new System.NotImplementedException();
        }

        public void Subscribe<T, TH>() where T : Event where TH : IEventHandler
        {
            throw new System.NotImplementedException();
        }
    }
}