using System;
using System.ComponentModel.DataAnnotations;

namespace CleanAzureServiceBus.Data.Entities
{
    public class MessageLog
    {
        [Key]
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string QueueName { get; set; }
        public MessageProcessResult ProcessResult { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetails { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ProcessedOn { get; set; }
        public string Body { get; set; }
        public long SequenceNumber { get; set; }
        public string MessageId { get; set; }
    }

    public enum MessageProcessResult
    {
        Success, InvalidMessage, UnexpectedError
    }
}