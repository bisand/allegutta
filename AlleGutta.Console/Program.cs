using AlleGutta.Nordnet;
using AlleGutta.Console;
using AlleGutta.Repository;
using AlleGutta.Repository.Database.Configuration;
using AlleGutta.Yahoo;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using AlleGutta.Portfolios.Models;

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

        var portfolio = new Portfolio()
        {
            Name = "AlleGutta",
            Ath = 0,
            Cash = data.AccountInfo?.AccountSum?.Value ?? 0,
            MarketValue = data.AccountInfo?.FullMarketvalue?.Value ?? 0,
            Positions = data.Positions?.Select(pos =>
            {
                return new PortfolioPosition()
                {
                    Symbol = pos.Instrument?.Symbol,
                    Name = pos.Instrument?.Name,
                    Shares = (int)pos.Qty,
                    AvgPrice = pos.AcqPrice?.Value ?? 0
                };
            }).ToArray()
        };

        var portfolioData = new PortfolioRepository(ConnectionString);
        var yahoo = new Yahoo();
        await portfolioData.SavePortfolioAsync(portfolio);

        portfolio = await portfolioData.GetPortfolioAsync("AlleGutta");
        var quotes = await yahoo.GetQuotes(portfolio.Positions.Select(x=>x.Symbol));
        Console.WriteLine(JsonConvert.SerializeObject(quotes));
    }
}