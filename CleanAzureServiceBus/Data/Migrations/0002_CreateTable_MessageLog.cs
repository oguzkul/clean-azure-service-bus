using CleanAzureServiceBus.Data.Entities;
using FluentMigrator;

namespace CleanAzureServiceBus.Data.Migrations
{
    [Migration(2)]
    public class _0002_CreateTable_MessageLog : Migration
    {
        public override void Up()
        {
            Create.Table(nameof(MessageLog))
                .WithColumn(nameof(MessageLog.Id)).AsInt32()
                    .PrimaryKey().Identity()
                .WithColumn(nameof(MessageLog.Guid)).AsGuid()
                .WithColumn(nameof(MessageLog.QueueName)).AsString(100)
                .WithColumn(nameof(MessageLog.ProcessResult)).AsInt32()
                .WithColumn(nameof(MessageLog.ErrorMessage)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(MessageLog.ErrorDetails)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(MessageLog.Body)).AsString(int.MaxValue)
                .WithColumn(nameof(MessageLog.CreatedOn)).AsDateTime2()
                .WithColumn(nameof(MessageLog.ProcessedOn)).AsDateTime2()
                .WithColumn(nameof(MessageLog.SequenceNumber)).AsInt64()
                .WithColumn(nameof(MessageLog.MessageId)).AsString(255);
        }

        public override void Down()
        {
            Delete.Table(nameof(MessageLog));
        }
    }
}