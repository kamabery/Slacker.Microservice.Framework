using System;

namespace Slacker.Microservice.Framework.EventBus
{
    public abstract class Event
    {
        public Event()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;           
        }

        public Guid Id { get; }

        public DateTime CreationDate { get; }
    }
}