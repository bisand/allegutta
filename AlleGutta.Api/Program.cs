using AlleGutta.Api;
using AlleGutta.Api.Hubs;
using AlleGutta.Api.Options;
using AlleGutta.Nordnet;
using AlleGutta.Nordnet.Models;
using AlleGutta.Portfolios;
using AlleGutta.Repository;
using AlleGutta.Repository.Database.Configuration;
using AlleGutta.Yahoo;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient(_ => new NordNetConfig("https://www.nordnet.no/login-next", username, password));
builder.Services.AddTransient<PortfolioRepository>();
builder.Services.AddTransient<YahooApi>();
builder.Services.AddTransient<NordnetWebScraper>();
builder.Services.AddTransient<PortfolioProcessor>();
builder.Services.AddHostedService<PortfolioWorker>();
builder.Services.AddSignalR();

builder.Services.AddSpaStaticFiles(config => config.RootPath = "/workspaces/allegutta/allegutta.web.app/build");

var app = builder.Build();

app.MapHub<PortfolioHub>("/hubs/portfolio");

var serviceProvider = app.Services;
var options = serviceProvider.GetService<IOptions<DatabaseOptions>>();
var connectionString = options?.Value.ConnectionString ?? string.Empty;

// Put the database update into a scope to ensure
// that all resources will be disposed.
var dbServiceProvider = DatabaseConfiguration.CreateServices(connectionString);
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
