using AlleGutta.Console;
using AlleGutta.Models;
using AlleGutta.Nordnet;
using AlleGutta.Repository;
using AlleGutta.Repository.Database.Configuration;
using AlleGutta.Yahoo;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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

        // var username = Environment.GetEnvironmentVariable("NORDNET_USERNAME") ?? string.Empty;
        // var password = Environment.GetEnvironmentVariable("NORDNET_PASSWORD") ?? string.Empty;

        // var nordnetProcessor = new NordnetWebScraper(new("https://www.nordnet.no/login-next", username, password));
        // var data = await nordnetProcessor.GetBatchData();

        // var portfolio = new Portfolio()
        // {
        //     Name = "AlleGutta",
        //     Ath = 0,
        //     Cash = data.AccountInfo?.account_sum?.value ?? 0,
        //     MarketValue = data.AccountInfo?.full_marketvalue?.value ?? 0,
        //     Positions = data.Positions?.Select(pos =>
        //     {
        //         return new PortfolioPosition()
        //         {
        //             Symbol = pos.instrument?.symbol,
        //             Name = pos.instrument?.name,
        //             Shares = (int)pos.qty,
        //             AvgPrice = pos.acq_price?.value ?? 0
        //         };
        //     }).ToArray()
        // };

        var portfolioData = new PortfolioRepository(ConnectionString);
        var yahoo = new Yahoo();
        // await portfolioData.SavePortfolioAsync(portfolio);

        var value = portfolioData.GetPortfolioPositionsAsync("AlleGutta");
        var positions = await value.ToListAsync();
        Portfolio portfolio = await yahoo.GetPortfolio(positions);
        Console.WriteLine(JsonConvert.SerializeObject(portfolio));
    }
}