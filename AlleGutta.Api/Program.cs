using AlleGutta.Api;
using AlleGutta.Nordnet;
using AlleGutta.Nordnet.Models;
using AlleGutta.Portfolios;
using AlleGutta.Repository;
using AlleGutta.Repository.Database.Configuration;
using AlleGutta.Yahoo;
using App.WorkerService;

const string connectionString = "Data Source=data/allegutta.db";
var builder = WebApplication.CreateBuilder(args);

var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, "../.env");
DotEnv.Load(dotenv);

// Add services to the container.

var username = Environment.GetEnvironmentVariable("NORDNET_USERNAME") ?? string.Empty;
var password = Environment.GetEnvironmentVariable("NORDNET_PASSWORD") ?? string.Empty;

builder.Services.AddControllersWithViews();
builder.Services.AddTransient(_ => new PortfolioRepository(connectionString));
builder.Services.AddTransient(_ => new NordNetConfig("https://www.nordnet.no/login-next", username, password));
builder.Services.AddTransient<Yahoo>();
builder.Services.AddTransient<NordnetWebScraper>();
builder.Services.AddTransient<PortfolioProcessor>();
builder.Services.AddHostedService<PortfolioWorker>();

var serviceProvider = DatabaseConfiguration.CreateServices(connectionString);

// Put the database update into a scope to ensure
// that all resources will be disposed.
using (var scope = serviceProvider.CreateScope())
{
    DatabaseConfiguration.UpdateDatabase(scope.ServiceProvider);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
