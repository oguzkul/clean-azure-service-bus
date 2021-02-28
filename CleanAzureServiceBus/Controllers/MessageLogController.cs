using System.Collections.Generic;
using System.Threading.Tasks;
using CleanAzureServiceBus.Data.Entities;
using CleanAzureServiceBus.Services;
using Microsoft.AspNetCore.Mvc;

namespace CleanAzureServiceBus.Controllers
{
    [Route("messageLog")]
    public class MessageLogController : ControllerBase
    {
        private readonly IMessageLogService _messageLogService;

        public MessageLogController(IMessageLogService messageLogService)
        {
            _messageLogService = messageLogService;
        }

        [HttpGet("")]
        public async Task<List<MessageLog>> GetMessageLogs()
        {
            var messageLogs = await _messageLogService.GetMessageLogsAsync();
            return messageLogs;
        }
    }
}