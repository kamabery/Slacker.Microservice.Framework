using System;
using System.IO;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Slacker.Microservice.Framework.EventBus.RabbitMQ
{
    public class RabbitMQConnection : IDisposable, IRabbitMQConnection
    {
        private readonly ILogger<RabbitMQConnection> logger;
        private ConnectionFactory factory;
        private IConnection connection;
        private readonly object sync_root = new object();
        private bool disposed;

        public RabbitMQConnection(ILogger<RabbitMQConnection> logger, RabbitMQOptions options)
        {
            this.logger = logger;
            lock (sync_root)
            {
                this.logger.LogInformation($"Setting up Connection Factory to Rabbit MQ Host: {options.HostName} User : {options.UserName}");
                if (options.Password.Length == 0)
                {
                    this.logger.LogInformation("Password is Empty");
                }

                this.QueueName = options.QueueName;

                this.Exchange = options.Exchange;

                this.factory = new ConnectionFactory
                {
                    HostName = options.HostName,
                    UserName = options.UserName,
                    Password = options.Password,
                    Port = options.Port

                };
            }

        }

        public string Exchange { get; }

        public string QueueName { get; }


        public bool TryConnect()
        {
            this.logger.LogInformation("Rabbit MQ Client is trying to Connect");

            lock (sync_root)
            {
                this.connection = this.factory.CreateConnection();
            }

            if (this.IsConnected)
            {
                this.logger.LogInformation($"Rabbit MQ Connected {this.connection.Endpoint}");
                return true;
            }

            this.logger.LogCritical($"Fatal Error: Rabbit MQ unable to Connect");
            return false;
        }

        public IModel CreateModel()
        {
            if (!this.IsConnected)
            {
                throw new InvalidOperationException("Rabbit MQ Connection is not open");
            }

            return this.connection.CreateModel();
        }


        public bool IsConnected
        {
            get { return this.connection != null && this.connection.IsOpen && !this.disposed; }
        }


        public void Dispose()
        {
            if (disposed) return;

            try
            {
                this.connection.Dispose();
            }
            catch (IOException e)
            {
                this.logger.LogCritical(e.ToString());
            }
        }
    }
}