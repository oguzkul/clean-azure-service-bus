namespace CleanAzureServiceBus.Exception
{
    //For exceptions caused by business logic and not server errors (like db failure)
    //These exceptions are handled in app itself instead of resending the message to the queue
    public class InvalidMessageException : System.Exception
    {
        public InvalidMessageException()
        {
        }

        public InvalidMessageException(string message)
            : base(message)
        {
        }

        public InvalidMessageException(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}