using System;
using Slacker.Microservice.Framework.EventBus;

namespace Slacker.Microserver.Sample.Events
{
    public class WorkItemLogged : Event
    {
        public WorkItemLogged(string subject, DateTime logDate)
        {
            Subject = subject;
            LogDate = logDate;
        }

        public string Subject { get; }

        public DateTime LogDate { get; }
    }
}