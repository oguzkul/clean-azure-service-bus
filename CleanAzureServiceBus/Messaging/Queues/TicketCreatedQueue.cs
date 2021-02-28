using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanAzureServiceBus.Messaging.Queues
{
    public class TicketCreatedQueue : ServiceBusQueue<TicketCreatedMessage>
    {
        public TicketCreatedQueue(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
            : base(configuration, serviceScopeFactory)
        { }
        protected override string QueueName => "TicketCreated";
    }

    public class TicketCreatedMessage : BaseMessage
    {
        public int TicketId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}