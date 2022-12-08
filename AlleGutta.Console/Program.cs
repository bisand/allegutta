using AlleGutta.Console;
using AlleGutta.Repository;
using AlleGutta.Repository.Database.Configuration;
using AlleGutta.Yahoo;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using AlleGutta.Portfolios;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AlleGutta.Nordnet.Models;
using AlleGutta.Nordnet;
using Microsoft.Extensions.Options;
using AlleGutta.Portfolios.Models;

static class Program
{
    private const string ConnectionString = "Data Source=data/allegutta.db";

    private static async Task Main(string[] args)
    {
        var root = Directory.GetCurrentDirectory();
        var dotenv = Path.Combine(root, "../.env");
        DotEnv.Load(dotenv);

        var username = Environment.GetEnvironmentVariable("NORDNET_USERNAME") ?? string.Empty;
        var password = Environment.GetEnvironmentVariable("NORDNET_PASSWORD") ?? string.Empty;

        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(DatabaseOptions.SectionName));

        builder.Services.AddTransient(_ => new NordNetConfig("https://www.nordnet.no/login-next", username, password));
        builder.Services.AddTransient<PortfolioRepository>();
        builder.Services.AddTransient<Yahoo>();
        builder.Services.AddTransient<NordnetWebScraper>();
        builder.Services.AddTransient<PortfolioProcessor>();

        var host = builder.Build();
        var serviceProvider = host.Services;
        var options = serviceProvider.GetService<IOptions<DatabaseOptions>>();

        var connectionString = options?.Value.ConnectionString ?? string.Empty;

        // Put the database update into a scope to ensure
        // that all resources will be disposed.
        var dbServiceProvider = DatabaseConfiguration.CreateServices(connectionString);
        using (var scope = dbServiceProvider.CreateScope())
        {
            DatabaseConfiguration.UpdateDatabase(scope.ServiceProvider);
        }

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

        var portfolioData = serviceProvider.GetService<PortfolioRepository>();
        var yahoo = serviceProvider.GetService<Yahoo>();

        await portfolioData.SavePortfolioAsync(portfolio);

        portfolio = await portfolioData.GetPortfolioAsync("AlleGutta");
        if (portfolio?.Positions is not null)
        {
            var quotes = await yahoo.GetQuotes(portfolio.Positions.Select(x => x.Symbol + ".OL"));
            portfolio = new PortfolioProcessor().Process(portfolio, quotes);
            await portfolioData.SavePortfolioAsync(portfolio);
            var chart = await yahoo.GetChartData("STL.OL", "1d", "1m");
            Console.WriteLine(JsonConvert.SerializeObject(chart));
        }

        Console.WriteLine(JsonConvert.SerializeObject(portfolio));
    }
}