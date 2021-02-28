using System;
using AutoMapper;
using CleanAzureServiceBus.Data.Entities;
using CleanAzureServiceBus.Messaging.Queues;

namespace CleanAzureServiceBus.Services
{
    public class ModelMapper : Profile
    {
        public ModelMapper()
        {
            //Setting special rule for createdOn, other fields are copied since they have same names
            CreateMap<NewTicketRequestedMessage, Ticket>()
                .ForMember(ticket => ticket.CreatedOn, source => source.MapFrom(i => DateTime.Now));
        }
    }
}