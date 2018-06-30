using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Slacker.Microservice.Framework.EventBus.RabbitMQ
{
    public static class Extensions
    {
        private static List<Action<IApplicationBuilder>> actions;

        public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                var configuration = sp.GetService<IConfiguration>();
                var options = new RabbitMQOptions();
                var section = configuration.GetSection("rabbitmq");
                section.Bind(options);
                var logger = sp.GetRequiredService<ILogger<RabbitMQConnection>>();
                var rabbitMQConnection = new RabbitMQConnection(logger, options);
                var eventLogger = sp.GetService<ILogger<RabbitMQEventBus>>();
                var subscriptionManagerLogger = sp.GetRequiredService<ILogger<SubscriptionManager>>();
                var subscriptionManager = new SubscriptionManager(sp, subscriptionManagerLogger);
                return new RabbitMQEventBus(rabbitMQConnection, eventLogger, subscriptionManager);
            });
           
            return services;
        }

        public static IServiceCollection RegisterEventHandler<T, TT, TH>(this IServiceCollection services)
            where TH: Event
            where TT : class, T
            where T : class, IEventHandler<TH>
        {
            services.AddTransient<T, TT>();
            if (actions == null)
            {
                actions = new List<Action<IApplicationBuilder>>();
            }
            actions.Add(HandleEvent<TH, T>);
            return services;
        }

        public static void SetupEventHandlers(this IApplicationBuilder app)
        {
            foreach (var action in actions)
            {
                action.Invoke(app);
            }
        }

        private static void HandleEvent<T, TT>(IApplicationBuilder app) 
            where T : Event 
            where TT : IEventHandler<T>
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();            
            eventBus.Subscribe<T>();
        }


    }
}