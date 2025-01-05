using AlleGutta.Api;
using AlleGutta.Api.Hubs;
using AlleGutta.Api.Options;
using AlleGutta.Common;
using AlleGutta.Nordnet;
using AlleGutta.Nordnet.Models;
using AlleGutta.Portfolios;
using AlleGutta.Repository;
using AlleGutta.Repository.Database.Configuration;
using AlleGutta.Yahoo;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;

var builder = WebApplication.CreateBuilder(args);

var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, "../.env");
DotEnv.Load(dotenv);

if (new[] { "NORDNET_USERNAME", "NORDNET_PASSWORD" }.Any(x => Environment.GetEnvironmentVariable(x)?.Length == 0))
    throw new ArgumentException("Missing Nordnet username or password. Use environment variables: NORDNET_USERNAME & NORDNET_PASSWORD");

var nordnetUsername = Environment.GetEnvironmentVariable("NORDNET_USERNAME") ?? throw new InvalidOperationException("NORDNET_USERNAME environment variable is missing");
var nordnetPassword = Environment.GetEnvironmentVariable("NORDNET_PASSWORD") ?? throw new InvalidOperationException("NORDNET_PASSWORD environment variable is missing");
var mariaDbPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? throw new InvalidOperationException("MYSQL_PASSWORD environment variable is missing");
var accountNoString = Environment.GetEnvironmentVariable("NORDNET_ACCOUNT") ?? throw new InvalidOperationException("NORDNET_ACCOUNT environment variable is missing");
if (!int.TryParse(accountNoString, out var accountNo))
{
    throw new InvalidOperationException("NORDNET_ACCOUNT environment variable is not a valid integer");
}

// Add services to the container.

builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection(WorkerOptions.SectionName));
builder.Services.Configure<DatabaseOptionsSQLite>(builder.Configuration.GetSection(DatabaseOptionsSQLite.SectionName));
builder.Services.Configure<DatabaseOptionsMariaDb>(builder.Configuration.GetSection(DatabaseOptionsMariaDb.SectionName));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient(_ => new NordNetConfig("https://www.nordnet.no/login-next", nordnetUsername, nordnetPassword, accountNo));
builder.Services.AddTransient<IPortfolioRepository, PortfolioRepositoryMariaDb>();
builder.Services.AddTransient<YahooApi>();
builder.Services.AddTransient<NordnetWebScraper>();
builder.Services.AddTransient<PortfolioProcessor>();
builder.Services.AddHostedService<PortfolioWorker>();
builder.Services.AddSignalR();

builder.Services.AddSpaStaticFiles(config => config.RootPath = "/workspaces/allegutta/allegutta.web.app/build");

var app = builder.Build();

app.MapHub<PortfolioHub>("/hubs/portfolio");

var serviceProvider = app.Services;
var repository = serviceProvider.GetService<IPortfolioRepository>();

// Put the database update into a scope to ensure
// that all resources will be disposed.
var dbServiceProvider = DatabaseConfiguration.CreateServices(repository?.ConnectionString ?? string.Empty);
using (var scope = dbServiceProvider.CreateScope())
{
    DatabaseConfiguration.UpdateDatabase(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapWhen(x => !(x?.Request?.Path.Value ?? string.Empty).StartsWith(new[] { "/api", "/hubs" }), builder =>
    {
        builder.UseSpa(spa =>
        {
            spa.Options.SourcePath = "/workspaces/allegutta/allegutta.web.app";
            spa.UseReactDevelopmentServer(npmScript: "start");
        });
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
