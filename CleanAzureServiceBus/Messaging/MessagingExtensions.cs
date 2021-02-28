using System.Text;
using CleanAzureServiceBus.Messaging.Queues;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;

namespace CleanAzureServiceBus.Messaging
{
    public static class MessagingExtensions
    {
        public static void AddServiceBusQueues(this IServiceCollection services)
        {
            // Add registered queues
            // IRegistrable for auto initializing
            services.AddSingleton<IRegisterable, NewTicketRequestedQueue>();
            services.AddSingleton<NewTicketRequestedQueue>();
            
            // Add non registered queues
            services.AddSingleton<TicketCreatedQueue>();
        }
        
        public static IApplicationBuilder UseServiceBusQueues(this IApplicationBuilder app)
        {
            var registeredServiceBusQueues = app.ApplicationServices.GetServices<IRegisterable>();
            foreach (var registeredServiceBusQueue in registeredServiceBusQueues)
                registeredServiceBusQueue.Register();
            return app;
        }
            
        public static string GetBodyAsString(this Message message)
        {
            return Encoding.UTF8.GetString(message.Body);
        }
    }
}