using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanAzureServiceBus.Data.Entities;

namespace CleanAzureServiceBus.Services
{
    public interface IMessageLogService
    {
        Task<List<MessageLog>> GetMessageLogsAsync(CancellationToken token = default);
    }
}