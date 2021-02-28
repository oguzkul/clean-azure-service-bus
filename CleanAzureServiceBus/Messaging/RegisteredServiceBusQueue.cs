using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CleanAzureServiceBus.Data;
using CleanAzureServiceBus.Data.Entities;
using CleanAzureServiceBus.Exception;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CleanAzureServiceBus.Messaging
{
public abstract class RegisteredServiceBusQueue<T> : ServiceBusQueue<T>, IRegisterable where T : BaseMessage
    {
        protected virtual BaseMessageValidator<T> ProcessMessageValidator { get; } = null;

        protected RegisteredServiceBusQueue(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory) :
            base(configuration, serviceScopeFactory) { }
        
        // Registers message handler (queue consumer) function
        public void Register(int maxConcurrentCalls = 1, bool autoComplete = false)
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = maxConcurrentCalls,
                AutoComplete = autoComplete
            };
            _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        // Method for handling new messages from the queue
        // This is called automatically when there is a new message in the queue
        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");
            
            var isMessageValid = true;
            var errorMessage = "";
            T model = default;
            var guid = Guid.Empty;
            try
            {
                model = JsonSerializer.Deserialize<T>(message.GetBodyAsString());
                // We are expecting every message to have a guid
                guid = model.Guid;
            }
            catch (System.Exception e)
            {
                isMessageValid = false;
                errorMessage = e.Message;
            }
            if (isMessageValid && ProcessMessageValidator != null)
            {
                var validationResult = await ProcessMessageValidator.ValidateAsync(model, token);
                isMessageValid = validationResult.IsValid;
                errorMessage = validationResult.ToString();
            }
            if (isMessageValid)
            {
                try
                {
                    await ProcessMessagesAsync(model, token);
                    await OnMessageSuccessfullyProcessedAsync(message, guid);
                }
                catch (InvalidMessageException e)
                {
                    isMessageValid = false;
                    errorMessage = e.Message;
                }
                catch (System.Exception e)
                {
                    await OnUnexpectedErrorOccuredAsync(message, guid, e);
                }
            }
            if (!isMessageValid)
            {
                await OnInvalidMessageReceivedAsync(message, guid, errorMessage);
            }
            
            // Complete the message so that it is not received again.
            // This can be done only if the queueClient is created in ReceiveMode.PeekLock mode (which is default).
            
            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);

            // Note: Use the cancellationToken passed as necessary to determine if the queueClient has already been closed.
            // If queueClient has already been Closed, you may chose to not call CompleteAsync() or AbandonAsync() etc. calls 
            // to avoid unnecessary exceptions.
        }

        // Modeled version of ProcessMessagesAsync for extended classes to override
        protected abstract Task ProcessMessagesAsync(T message, CancellationToken token);

        // This runs if ProcessMessagesAsync throws exception other than InvalidMessageException
        protected virtual Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

        protected virtual async Task OnMessageSuccessfullyProcessedAsync(Message message, Guid guid, List<string> updates = null)
        {
            await LogProcessedMessage(message, MessageProcessResult.Success, guid);
        }
        
        protected virtual async Task OnUnexpectedErrorOccuredAsync(Message message, Guid guid, System.Exception e)
        {
            await LogProcessedMessage(message, MessageProcessResult.UnexpectedError, guid, e.Message, e.ToString());
        }

        // Logs invalid messages
        // Called if type of message is invalid or validator fails or InvalidMessageException has thrown
        protected virtual async Task OnInvalidMessageReceivedAsync(Message message, Guid guid, string errorMessage)
        {
            await LogProcessedMessage(message, MessageProcessResult.InvalidMessage, guid, errorMessage);
        }
        
        private async Task LogProcessedMessage(Message message, MessageProcessResult result, Guid guid, string errorMessage = null, string errorDetails = null)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                await dataContext.MessageLog.AddAsync(new MessageLog
                {
                    Guid = guid,
                    CreatedOn = message.SystemProperties.EnqueuedTimeUtc,
                    ProcessedOn = DateTime.Now,
                    Body = message.GetBodyAsString(),
                    ProcessResult = result,
                    ErrorMessage = errorMessage,
                    ErrorDetails = errorDetails,
                    QueueName = QueueName,
                    SequenceNumber = message.SystemProperties.SequenceNumber,
                    MessageId = message.MessageId
                });
                
                await dataContext.SaveChangesAsync();
            }
        }
    }
}