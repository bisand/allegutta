using System;
using AlleGutta.Console;
using AlleGutta.Console.Database;
using AlleGutta.Nordnet;
using AlleGutta.Repository;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var root = Directory.GetCurrentDirectory();
        var dotenv = Path.Combine(root, "../.env");
        DotEnv.Load(dotenv);

        var serviceProvider = CreateServices();

        // Put the database update into a scope to ensure
        // that all resources will be disposed.
        using (var scope = serviceProvider.CreateScope())
        {
            UpdateDatabase(scope.ServiceProvider);
        }

        var username = Environment.GetEnvironmentVariable("NORDNET_USERNAME") ?? string.Empty;
        var password = Environment.GetEnvironmentVariable("NORDNET_PASSWORD") ?? string.Empty;

        // var nordnetProcessor = new NordnetWebScraper(new("https://www.nordnet.no/login-next", username, password));
        // var data = await nordnetProcessor.GetBatchData();

        var portfolioData = new PortfolioData("Data Source=data/allegutta.db");
        var value = portfolioData.GetPortfolioPositionsAsync("AlleGutta");
        var valueTask = await value.ToListAsync();
        Console.WriteLine(valueTask);
    }

    /// <summary>
    /// Configure the dependency injection services
    /// </summary>
    private static IServiceProvider CreateServices()
    {
        return new ServiceCollection()
            // Add common FluentMigrator services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                // Add SQLite support to FluentMigrator
                .AddSQLite()
                // Set the connection string
                .WithGlobalConnectionString("Data Source=data/test.db")
                // Define the assembly containing the migrations
                .ScanIn(typeof(AddLogTable).Assembly).For.Migrations())
            // Enable logging to console in the FluentMigrator way
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            // Build the service provider
            .BuildServiceProvider(false);
    }

    /// <summary>
    /// Update the database
    /// </summary>
    private static void UpdateDatabase(IServiceProvider serviceProvider)
    {
        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        // Execute the migrations
        runner.MigrateUp();
    }

}