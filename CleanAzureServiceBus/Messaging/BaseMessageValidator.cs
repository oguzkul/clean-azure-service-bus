using System;
using System.Threading;
using System.Threading.Tasks;
using CleanAzureServiceBus.Exception;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CleanAzureServiceBus.Messaging
{
    public class BaseMessageValidator<T> : AbstractValidator<T> where T: BaseMessage
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public BaseMessageValidator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

            RuleFor(i => i.Guid)
                .NotEqual(Guid.Empty);
        }
        
        public BaseMessageValidator()
        {
            RuleFor(i => i.Guid)
                .NotEqual(Guid.Empty);
        }

        public async Task ValidateAndThrowExceptionAsync(T instance)
        {
            await ValidateAndThrowExceptionAsync(instance, CancellationToken.None);
        }

        public async Task ValidateAndThrowExceptionAsync(T instance, CancellationToken token)
        {
            var validationResult = await ValidateAsync(instance, token);
            if (!validationResult.IsValid)
            {
                throw new FluentValidationException { ValidationResult = validationResult };
            }
        }
    }
}