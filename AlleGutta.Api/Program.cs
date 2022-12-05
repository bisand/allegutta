using AlleGutta.Repository;
using AlleGutta.Repository.Database.Configuration;

const string connectionString = "Data Source=data/allegutta.db";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddTransient(_ => new PortfolioRepository(connectionString));

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
