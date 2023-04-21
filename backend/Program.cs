using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZapMe.Constants;
using ZapMe.Controllers;
using ZapMe.Data;
using ZapMe.Middlewares;

// The services are ordered by dependency requirements.
// The middlewares are ordered by execution order.

// ########################################
// ######## GLOBAL SETTINGS ###############
// ########################################

// Set global Regex timeout to 1 second
AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(1000));

// ########################################
// ######## CREATE BUILDER ################
// ########################################

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IWebHostEnvironment env = builder.Environment;
IServiceCollection services = builder.Services;
bool isDevelopment = env.IsDevelopment();

// ########################################
// ######## CONFIGURE CONFIGURATION #######
// ########################################

builder.Configuration.AddUserSecrets<Program>();
IConfiguration configuration = builder.Configuration;
bool isBuild = configuration.GetValue<bool>("IsBuild", false);

// ########################################
// ######## CONFIGURE LOGGING #############
// ########################################

IdentityModelEventSource.ShowPII = isDevelopment;
builder.Logging.AddSimpleConsole(opt =>
{
    opt.IncludeScopes = true;
    opt.SingleLine = false;
    opt.UseUtcTimestamp = true;
    opt.TimestampFormat = "[yyyy-MM-dd HH:mm:ss.fff] ";
});

// ########################################
// ######## CORE SERVICES #################
// ########################################

services.AddRouting();
services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false));
});
//services.AddHealthChecks().AddCheck("sql" ) //TODO: explore this
services.Configure<ApiBehaviorOptions>(opt =>
{
    opt.InvalidModelStateResponseFactory = ErrorResponseFactory.CreateErrorResult;
});
services.AddResponseCaching(opt =>
{
    opt.MaximumBodySize = 64 * 1024;
});

// ########################################
// ######## ZAPME SERVICES ################
// ########################################

services.ZMAddHttpClients(configuration);

services.ZMAddUsers();
services.ZMAddSessions();
services.ZMAddLockOuts();
services.ZMAddUserRelations();
services.ZMAddFriendRequests();
services.ZMAddEmailTemplates();

services.ZMAddWebSockets();
services.ZMAddRateLimiter();
services.ZMAddSwagger();
services.ZMAddAuthentication(configuration);
services.ZMAddAuthorization();
services.ZMAddDatabase(configuration);
services.ZMAddAmazonAWS(configuration);
services.ZMAddQuartz();

// ########################################
// ######## CORS CONFIGURATION ############
// ########################################

services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder =>
    {
        if (isDevelopment)
        {
            builder.WithOrigins("http://localhost:5173");
        }
        else
        {
            builder.WithOrigins("https://zapme.app", "https://www.zapme.app");
        }
        builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

// ########################################
// ######## BUILD APP #####################
// ########################################

WebApplication app = builder.Build();

// ########################################
// ######## SET UP DATABASE ###############
// ########################################

if (!isBuild)
{
    using (IServiceScope scope = app.Services.CreateScope())
    {
        IServiceProvider serviceProvider = scope.ServiceProvider;

        if (!((serviceProvider.GetService<IDatabaseCreator>() as RelationalDatabaseCreator)?.Exists() ?? false))
        {
            ZapMeContext context = serviceProvider.GetRequiredService<ZapMeContext>();
            await context.Database.MigrateAsync();
            await DataSeeders.SeedAsync(context);
        }
    }
}
else
{
    Console.WriteLine("Project is being built, skipping database setup.");
}

// ########################################
// ######## MIDDLEWARE PIPELINE ###########
// ########################################

if (isDevelopment)
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.Map("/api", true, app =>
{
    // App!.UseHealthChecks("/api/v1/health/"); // TODO: explore this

    app.UseRouting();
    app.UseCors(); // Use default policy
    app.UseAuthorization();
    app.UseRateLimiter();
    app.UseMiddleware<ActivityTracker>();
    app.UseEndpoints(endpoints => endpoints.MapControllers());
});
app.Map("/swagger", true, app =>
{
    app.UseHeaderValue("Cache-Control", "public, max-age=86400");
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/swagger/v1/swagger.json", App.AppName + " API - json");
        opt.SwaggerEndpoint("/swagger/v1/swagger.yaml", App.AppName + " API - yaml");
        opt.InjectStylesheet("/static/SwaggerDark.css");
    });
});
app.Map("/static", false, app =>
{
    app.UseResponseCaching();
    app.UseStaticFiles();
});

// ########################################
// ######## RUN APP #######################
// ########################################

app.Run();