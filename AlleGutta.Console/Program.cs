using AlleGutta.Console;
using AlleGutta.Repository;
using AlleGutta.Repository.Database.Configuration;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var root = Directory.GetCurrentDirectory();
        var dotenv = Path.Combine(root, "../.env");
        DotEnv.Load(dotenv);

        var serviceProvider = DatabaseConfiguration.CreateServices();

        // Put the database update into a scope to ensure
        // that all resources will be disposed.
        using (var scope = serviceProvider.CreateScope())
        {
            DatabaseConfiguration.UpdateDatabase(scope.ServiceProvider);
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

}