using RabbitMQ.Client;

namespace Slacker.Microservice.Framework.EventBus.RabbitMQ
{
    public interface IRabbitMQConnection
    {
        string Exchange { get; }
        string QueueName { get; }
        bool IsConnected { get; }
        bool TryConnect();
        IModel CreateModel();
        void Dispose();
    }
}