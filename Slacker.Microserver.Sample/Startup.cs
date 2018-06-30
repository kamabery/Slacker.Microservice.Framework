using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Slacker.Microserver.Sample.EventHandlers;
using Slacker.Microserver.Sample.Events;
using Slacker.Microservice.Framework.EventBus;
using Slacker.Microservice.Framework.EventBus.RabbitMQ;

namespace Slacker.Microserver.Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddRabbitMQ();
            services.RegisterEventHandler<IEventHandler<WorkItemAdded>, WorkItemAddedEventHandler, WorkItemAdded>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.SetupEventHandlers();
            app.UseMvc();
        }
    }
}
