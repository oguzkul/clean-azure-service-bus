using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CleanAzureServiceBus.Messaging
{
    public abstract class ServiceBusQueue<T> where T : BaseMessage
    {
        // ServiceScopeFactory is needed for using non-singleton services when handling messages
        protected readonly IServiceScopeFactory _serviceScopeFactory;
        protected readonly IQueueClient _queueClient;
        protected abstract string QueueName { get; }
        protected virtual BaseMessageValidator<T> SendMessageValidator { get; } = null;

        protected ServiceBusQueue(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            // This should be a secret
            var connectionString = configuration.GetSection("AzureServiceBus").GetSection("ConnectionString").Get<string>();
            _queueClient = new QueueClient(connectionString, QueueName);
        }
        
        // Send new message to the queue
        public async Task SendMessageAsync(T model)
        {
            if (SendMessageValidator != null)
                await SendMessageValidator.ValidateAndThrowExceptionAsync(model);
            var body = JsonSerializer.Serialize(model);
            await _queueClient.SendAsync(new Message(Encoding.UTF8.GetBytes(body)));
        }
    }
}