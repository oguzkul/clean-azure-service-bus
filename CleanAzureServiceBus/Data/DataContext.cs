using CleanAzureServiceBus.Data.Entities;
using Microsoft.EntityFrameworkCore;

 namespace CleanAzureServiceBus.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> dbContextOptions) : base(dbContextOptions) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Ticket> Ticket { get; set; }
        public virtual DbSet<MessageLog> MessageLog { get; set; }
    }
}