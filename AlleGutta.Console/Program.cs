using System;
using AlleGutta.Nordnet;
using AlleGutta.Repository;

var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, "../.env");
DotEnv.Load(dotenv);

var username = Environment.GetEnvironmentVariable("NORDNET_USERNAME") ?? string.Empty;
var password = Environment.GetEnvironmentVariable("NORDNET_PASSWORD") ?? string.Empty;

// var nordnetProcessor = new NordnetWebScraper(new("https://www.nordnet.no/login-next", username, password));
// var data = await nordnetProcessor.GetBatchData();

var portfolioData = new PortfolioData();
portfolioData.GetData("10");
Console.WriteLine("data");
