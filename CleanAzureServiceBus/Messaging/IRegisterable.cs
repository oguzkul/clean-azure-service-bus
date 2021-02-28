namespace CleanAzureServiceBus.Messaging
{
    public interface IRegisterable
    {
        public void Register(int maxConcurrentCalls = 1, bool autoComplete = false);
    }
}