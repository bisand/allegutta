using AlleGutta.Api;
using AlleGutta.Nordnet;
using AlleGutta.Nordnet.Models;
using AlleGutta.Portfolios;
using AlleGutta.Repository;
using AlleGutta.Repository.Database.Configuration;
using AlleGutta.Yahoo;
using App.WorkerService;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, "../.env");
DotEnv.Load(dotenv);

if (new[] { "NORDNET_USERNAME", "NORDNET_PASSWORD" }.Any(x => Environment.GetEnvironmentVariable(x)?.Length == 0))
    throw new ArgumentException("Missing Nordnet username or password. Use environment variables: NORDNET_USERNAME & NORDNET_PASSWORD");

var username = Environment.GetEnvironmentVariable("NORDNET_USERNAME") ?? string.Empty;
var password = Environment.GetEnvironmentVariable("NORDNET_PASSWORD") ?? string.Empty;

// Add services to the container.
builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection(WorkerOptions.SectionName));
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(DatabaseOptions.SectionName));

builder.Services.AddControllersWithViews();
builder.Services.AddTransient(_ => new NordNetConfig("https://www.nordnet.no/login-next", username, password));
builder.Services.AddTransient<PortfolioRepository>();
builder.Services.AddTransient<YahooApi>();
builder.Services.AddTransient<NordnetWebScraper>();
builder.Services.AddTransient<PortfolioProcessor>();
builder.Services.AddHostedService<PortfolioWorker>();
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
