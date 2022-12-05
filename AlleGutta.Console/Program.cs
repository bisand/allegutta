using AlleGutta.Console;
using AlleGutta.Nordnet;
using AlleGutta.Repository;
using AlleGutta.Repository.Database.Configuration;
using Microsoft.Extensions.DependencyInjection;

internal static class Program
{
    private const string ConnectionString = "Data Source=data/allegutta.db";

    private static async Task Main(string[] args)
    {
        var root = Directory.GetCurrentDirectory();
        var dotenv = Path.Combine(root, "../.env");
        DotEnv.Load(dotenv);

        var serviceProvider = DatabaseConfiguration.CreateServices(ConnectionString);

        // Put the database update into a scope to ensure
        // that all resources will be disposed.
        using (var scope = serviceProvider.CreateScope())
        {
            DatabaseConfiguration.UpdateDatabase(scope.ServiceProvider);
        }

        var username = Environment.GetEnvironmentVariable("NORDNET_USERNAME") ?? string.Empty;
        var password = Environment.GetEnvironmentVariable("NORDNET_PASSWORD") ?? string.Empty;

        var nordnetProcessor = new NordnetWebScraper(new("https://www.nordnet.no/login-next", username, password));
        var data = await nordnetProcessor.GetBatchData();

        var portfolioData = new PortfolioData(ConnectionString);
        var value = portfolioData.GetPortfolioPositionsAsync("AlleGutta");
        var valueTask = await value.ToListAsync();
        Console.WriteLine(valueTask);
    }
}