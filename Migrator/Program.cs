using Microsoft.EntityFrameworkCore;
using ZapMe.Database;
using ZapMe.Database.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddZapMeDatabase(builder.Configuration);

var app = builder.Build();

using IServiceScope scope = app.Services.CreateScope();
DatabaseContext context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

// Create database if not exists
if (context.Database.EnsureCreated())
{
    // If database was created, seed it
    await DatabaseSeeders.SeedAsync(context);
}
else
{
    // Apply migrations
    await context.Database.MigrateAsync();
}

app.Run();