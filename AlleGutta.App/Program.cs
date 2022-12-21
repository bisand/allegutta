﻿using AlleGutta.Repository;
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

        var username = Environment.GetEnvironmentVariable("NORDNET_USERNAME") ?? string.Empty;
        var password = Environment.GetEnvironmentVariable("NORDNET_PASSWORD") ?? string.Empty;

        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(DatabaseOptions.SectionName));

        builder.Services.AddTransient(_ => new NordNetConfig("https://www.nordnet.no/login-next", username, password));
        builder.Services.AddTransient<PortfolioRepositorySQLite>();
        builder.Services.AddTransient<YahooApi>();
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

        var portfolioProcessor = serviceProvider.GetService<PortfolioProcessor>() ?? throw new NullReferenceException($"Missing registration: {nameof(PortfolioProcessor)}");
        var nordnetProcessor = serviceProvider.GetService<NordnetWebScraper>() ?? throw new NullReferenceException($"Missing registration: {nameof(NordnetWebScraper)}");
        var portfolioData = serviceProvider.GetService<PortfolioRepositorySQLite>() ?? throw new NullReferenceException($"Missing registration: {nameof(PortfolioRepositorySQLite)}");
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