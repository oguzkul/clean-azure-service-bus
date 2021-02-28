using System.Collections.Generic;
using System.Threading.Tasks;
using CleanAzureServiceBus.Data.Entities;
using CleanAzureServiceBus.Messaging.Queues;
using CleanAzureServiceBus.Services;
using Microsoft.AspNetCore.Mvc;

namespace CleanAzureServiceBus.Controllers
{
    // This controller is for testing purposes you don't have to write a controller to use Azure Service Bus
    [Route("ticket")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly NewTicketRequestedQueue _newTicketRequestedQueue;

        public TicketController(ITicketService ticketService, NewTicketRequestedQueue newTicketRequestedQueue)
        {
            _ticketService = ticketService;
            _newTicketRequestedQueue = newTicketRequestedQueue;
        }

        [HttpGet("")]
        public async Task<List<Ticket>> GetTickets()
        {
            var tickets = await _ticketService.GetTicketsAsync();
            return tickets;
        }

        [HttpGet("{id}")]
        public async Task<Ticket> GetTicket([FromRoute] int id)
        {
            var ticket = await _ticketService.GetTicketAsync(id);
            return ticket;
        }

        [HttpPost("")]
        public async Task<IActionResult> SendMessageToNewTicketRequestedQueue([FromBody] NewTicketRequestedMessage message)
        {
            await _newTicketRequestedQueue.SendMessageAsync(message);
            return Ok();
        }
    }
}