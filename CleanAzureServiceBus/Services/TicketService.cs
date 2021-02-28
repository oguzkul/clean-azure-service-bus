using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanAzureServiceBus.Data;
using CleanAzureServiceBus.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanAzureServiceBus.Services
{
    public class TicketService : ITicketService
    {
        private readonly DataContext _dataContext;

        public TicketService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Ticket> GetTicketAsync(int id)
        {
            var ticket = await _dataContext.Ticket.FirstOrDefaultAsync(i => i.Id == id);
            return ticket;
        }

        public async Task<List<Ticket>> GetTicketsAsync(CancellationToken token = default)
        {
            var tickets = await _dataContext.Ticket
                .OrderByDescending(i => i.Id)
                .ToListAsync(token);
            return tickets;
        }

        public async Task<Ticket> CreateTicketAsync(Ticket ticket, CancellationToken token = default)
        {
            await _dataContext.Ticket.AddAsync(ticket, token);
            await _dataContext.SaveChangesAsync(token);
            return ticket;
        }
    }
}