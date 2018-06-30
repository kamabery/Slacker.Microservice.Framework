using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Slacker.Microservice.Framework.EventBus.RabbitMQ
{
    public class RabbitMQEventBus : IEventBus
    {
        private readonly IRabbitMQConnection connection;
        private readonly ILogger<RabbitMQEventBus> logger;
        private readonly ISubscriptionManager subscriptionManager;
        private readonly string queueName;
        private readonly string exchange;
        private static IModel consumerChannel;

        public RabbitMQEventBus(IRabbitMQConnection connection, ILogger<RabbitMQEventBus> logger, ISubscriptionManager subscriptionManager)
        {
            this.connection = connection;
            this.logger = logger;
            this.subscriptionManager = subscriptionManager;
            this.queueName = connection.QueueName;
            this.exchange = connection.Exchange;
        }


        public bool Publish(Event @event)
        {
            var routingKey = @event.GetType().Name;
            if (!this.connection.IsConnected)
            {
                if (!this.connection.TryConnect())
                {
                    this.logger.LogCritical(
                        $"Unable to to Publish Event ID {@event.Id} of Type {routingKey} Unable to connect to Queue");
                    return false;
                }
            }

            using (var channel = this.connection.CreateModel())
            {
                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);
                var basicProperties = channel.CreateBasicProperties();
                basicProperties.DeliveryMode = 2; // persistent
                channel.BasicPublish(
                    this.exchange,
                    routingKey,
                    true, // Mandatory
                    basicProperties,
                    body);

                this.logger.LogInformation($"Published message - {message} on exchange {this.exchange}");
            }

            return true;
        }

        public bool Subscribe<T>() where T : Event
        {
            var routingKey = typeof(T).Name;
            if (!this.connection.TryConnect())
            {
                this.logger.LogCritical($"Unable to Subsrcibe to Event {routingKey} Cannot Connect to Rabbit MQ");
                return false;
            }

            this.subscriptionManager.AddEvent(typeof(T));

            if (consumerChannel == null)
            {
                SetupConsumer(routingKey);
            }

            return true;
        }

        private void SetupConsumer(string routingKey)
        {
            consumerChannel = this.connection.CreateModel();
            consumerChannel.ExchangeDeclare(this.exchange, ExchangeType.Direct);
            consumerChannel.QueueDeclare(queueName, false, false, false, null);
            consumerChannel.QueueBind(this.queueName, this.exchange, routingKey);
            var consumer = new EventingBasicConsumer(consumerChannel);
            consumer.Received += async (model, ea) =>
            {
                this.logger.LogInformation($"Message Received {ea.RoutingKey}");
                var eventName = ea.RoutingKey;
                var message = Encoding.UTF8.GetString(ea.Body);

                await ProcessEvent(eventName, message);
                consumerChannel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            consumerChannel.BasicConsume(this.queueName, true, consumer);
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            await this.subscriptionManager.HandleMessage(eventName, message);
        }
    }
}