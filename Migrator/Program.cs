using Microsoft.EntityFrameworkCore;
using ZapMe.Database;
using ZapMe.Database.Extensions;
using ZapMe.Services;
using ZapMe.Services.Interfaces;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddZapMeDatabase(builder.Configuration);

var app = builder.Build();

using IServiceScope scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;
DatabaseContext context = serviceProvider.GetRequiredService<DatabaseContext>();

// Create database if not exists
if (context.Database.EnsureCreated())
{
    // If database was created, seed it
    DatabaseSeeder seeder = new(context, serviceProvider);
    await seeder.SeedAsync();
}
else
{
    // Apply migrations
    await context.Database.MigrateAsync();
}

app.Run();