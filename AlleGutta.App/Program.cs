using AlleGutta.Repository;
using AlleGutta.Repository.Database.Configuration;
using AlleGutta.Yahoo;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using AlleGutta.Portfolios;
using Microsoft.Extensions.Hosting;
using AlleGutta.Nordnet.Models;
using AlleGutta.Nordnet;
using Microsoft.Extensions.Options;

namespace AlleGutta.App;

static class Program
{
    private static async Task Main(string[] args)
    {
        var root = Directory.GetCurrentDirectory();
        var dotenv = Path.Combine(root, "../.env");
        DotEnv.Load(dotenv);

        var username = Environment.GetEnvironmentVariable("NORDNET_USERNAME") ?? throw new InvalidOperationException("NORDNET_USERNAME environment variable is missing");
        var password = Environment.GetEnvironmentVariable("NORDNET_PASSWORD") ?? throw new InvalidOperationException("NORDNET_PASSWORD environment variable is missing");
        var accountNoString = Environment.GetEnvironmentVariable("NORDNET_ACCOUNT") ?? throw new InvalidOperationException("NORDNET_ACCOUNT environment variable is missing");
        if (!int.TryParse(accountNoString, out var accountNo))
        {
            throw new InvalidOperationException("NORDNET_ACCOUNT environment variable is not a valid integer");
        }

        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.Configure<DatabaseOptionsSQLite>(builder.Configuration.GetSection(DatabaseOptionsSQLite.SectionName));

        builder.Services.AddTransient(_ => new NordNetConfig("https://www.nordnet.no/login-next", username, password, accountNo));
        builder.Services.AddTransient<IPortfolioRepository, PortfolioRepositoryMariaDb>();
        builder.Services.AddTransient<YahooApi>();
        builder.Services.AddTransient<NordnetWebScraper>();
        builder.Services.AddTransient<PortfolioProcessor>();

        var host = builder.Build();
        var serviceProvider = host.Services;
        var options = serviceProvider.GetService<IOptions<DatabaseOptionsSQLite>>();

        var connectionString = options?.Value.ConnectionString ?? string.Empty;

        // Put the database update into a scope to ensure
        // that all resources will be disposed.
        var dbServiceProvider = DatabaseConfiguration.CreateServices(connectionString);
        using (var scope = dbServiceProvider.CreateScope())
        {
            DatabaseConfiguration.UpdateDatabase(scope.ServiceProvider);
        }

        var portfolioProcessor = serviceProvider.GetService<PortfolioProcessor>() ?? throw new NullReferenceException($"Missing registration: {nameof(PortfolioProcessor)}");
        var nordnetProcessor = serviceProvider.GetService<NordnetWebScraper>() ?? throw new NullReferenceException($"Missing registration: {nameof(NordnetWebScraper)}");
        var portfolioData = serviceProvider.GetService<IPortfolioRepository>() ?? throw new NullReferenceException($"Missing registration: {nameof(IPortfolioRepository)}");
        var yahoo = serviceProvider.GetService<YahooApi>() ?? throw new NullReferenceException($"Missing registration: {nameof(Yahoo)}");

        var batchData = await nordnetProcessor.GetBatchData();
        var nordnetPortfolio = portfolioProcessor.GetPortfolioFromBatchData("AlleGutta", batchData);
        await portfolioData.SavePortfolioAsync(nordnetPortfolio);

        var portfolio = await portfolioData.GetPortfolioAsync("AlleGutta");
        if (portfolio?.Positions is not null)
        {
            var quotes = await yahoo.GetQuotes(portfolio.Positions.Select(x => x.Symbol + ".OL"));
            portfolio = portfolioProcessor.UpdatePortfolioWithMarketData(portfolio, quotes);
            await portfolioData.SavePortfolioAsync(portfolio);
            var chart = await yahoo.GetChartData("STL.OL", "1d", "1m");
            Console.WriteLine(JsonConvert.SerializeObject(chart));
        }

        Console.WriteLine(JsonConvert.SerializeObject(portfolio));
    }
}