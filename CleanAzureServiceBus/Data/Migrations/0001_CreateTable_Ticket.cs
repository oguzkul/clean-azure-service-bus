using CleanAzureServiceBus.Data.Entities;
using FluentMigrator;

namespace CleanAzureServiceBus.Data.Migrations
{
    [Migration(1)]
    public class _0001_CreateTable_Ticket : Migration
    {
        public override void Up()
        {
            Create.Table(nameof(Ticket))
                .WithColumn(nameof(Ticket.Id)).AsInt32()
                    .PrimaryKey().Identity()
                .WithColumn(nameof(Ticket.UserId)).AsInt32()
                .WithColumn(nameof(Ticket.Title)).AsString(255)
                .WithColumn(nameof(Ticket.Description)).AsString(int.MaxValue).Nullable()
                .WithColumn(nameof(Ticket.CreatedOn)).AsDateTime2();
        }

         public override void Down()
        {
            Delete.Table(nameof(Ticket));
        }
    }
}