using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanAzureServiceBus.Data.Entities;

namespace CleanAzureServiceBus.Services
{
    public interface ITicketService
    {
        Task<Ticket> GetTicketAsync(int id);
        Task<List<Ticket>> GetTicketsAsync(CancellationToken token = default);
        Task<Ticket> CreateTicketAsync(Ticket ticket, CancellationToken token = default);
    }
}