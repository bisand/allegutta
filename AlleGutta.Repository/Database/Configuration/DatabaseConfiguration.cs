using AlleGutta.Repository.Database;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace AlleGutta.Repository.Database.Configuration;

public static class DatabaseConfiguration
{
    /// <summary>
    /// Configure the dependency injection services
    /// </summary>
    public static IServiceProvider CreateServices()
    {
        return new ServiceCollection()
            // Add common FluentMigrator services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                // Add SQLite support to FluentMigrator
                .AddSQLite()
                // Set the connection string
                .WithGlobalConnectionString("Data Source=data/allegutta.db")
                // Define the assembly containing the migrations
                .ScanIn(new[] {
                    typeof(CreatePortfolioTable).Assembly,
                    typeof(CreatePortfolioPositionsTable).Assembly
                }).For.Migrations())
            // Enable logging to console in the FluentMigrator way
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            // Build the service provider
            .BuildServiceProvider(false);
    }

    /// <summary>
    /// Update the database
    /// </summary>
    public static void UpdateDatabase(IServiceProvider serviceProvider)
    {
        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        // Execute the migrations
        runner.MigrateUp();
    }

}
