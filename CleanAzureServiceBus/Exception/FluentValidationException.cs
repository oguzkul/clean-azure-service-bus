using FluentValidation.Results;

namespace CleanAzureServiceBus.Exception
{
    public class FluentValidationException : System.Exception
    {
        public ValidationResult ValidationResult { get; set; }
    }
}