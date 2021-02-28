using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanAzureServiceBus.Data;
using CleanAzureServiceBus.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanAzureServiceBus.Services
{
    public class MessageLogService : IMessageLogService
    {
        private readonly DataContext _dataContext;

        public MessageLogService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<MessageLog>> GetMessageLogsAsync(CancellationToken token = default)
        {
            var messageLogs = await _dataContext.MessageLog
                .OrderByDescending(i => i.Id)
                .ToListAsync(token);
            return messageLogs;
        }
    }
}