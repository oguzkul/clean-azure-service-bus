using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanAzureServiceBus.Data.Entities;
using CleanAzureServiceBus.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanAzureServiceBus.Messaging.Queues
{
    //This queue should be consumed by another service
    public class NewTicketRequestedQueue : RegisteredServiceBusQueue<NewTicketRequestedMessage>
    {
        private readonly TicketCreatedQueue _ticketCreatedQueue;
        private readonly IMapper _mapper;

        public NewTicketRequestedQueue(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, TicketCreatedQueue ticketCreatedQueue, IMapper mapper)
            : base(configuration, serviceScopeFactory)
        {
            _ticketCreatedQueue = ticketCreatedQueue;
            _mapper = mapper;
        }

        protected override string QueueName => "NewTicketRequested";
        protected override BaseMessageValidator<NewTicketRequestedMessage> ProcessMessageValidator =>
            new NewRequestCreatedMessageValidator(_serviceScopeFactory);
        
        protected override async Task ProcessMessagesAsync(NewTicketRequestedMessage message, CancellationToken token)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var ticketService = scope.ServiceProvider.GetRequiredService<ITicketService>();
                //Mapper automatically sets fields with same name
                var ticket = _mapper.Map<Ticket>(message);
                await ticketService.CreateTicketAsync(ticket, token);

                await _ticketCreatedQueue.SendMessageAsync(new TicketCreatedMessage
                {
                    Guid = message.Guid,
                    TicketId = ticket.Id,
                    CreatedOn = ticket.CreatedOn
                });
            }
        }
    }

    public class NewTicketRequestedMessage : BaseMessage
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
    
    public class NewRequestCreatedMessageValidator : BaseMessageValidator<NewTicketRequestedMessage>
    {
        public NewRequestCreatedMessageValidator(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
            RuleFor(x => x.Title)
                .NotEmpty();
        }
    }
}