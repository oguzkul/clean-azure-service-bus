using System;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanAzureServiceBus.Data
{
    public static class ConfigureMigration
    {
        public static void UseMigration(this IApplicationBuilder app)
        {
            var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("clean-azure-service-bus");
            var serviceProvider = CreateServices(connectionString);
            
            using (var scope = serviceProvider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetService<IMigrationRunner>();
                // Don't try uncommenting this at home
                //runner.MigrateDown(0);
                runner.MigrateUp();
            }
        }

        private static IServiceProvider CreateServices(string dbConnectionString)
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer()
                    .WithGlobalConnectionString(dbConnectionString)
                    .ScanIn(typeof(ConfigureMigration).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }
    }
}
