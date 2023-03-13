using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
ILoggingBuilder logging = builder.Logging;
IServiceCollection services = builder.Services;
IConfiguration configuration = builder.Configuration;
bool isDevelopment = env.IsDevelopment();

// ########################################
// ######## CONFIGURE LOGGING #############
// ########################################

IdentityModelEventSource.ShowPII = isDevelopment;
logging.AddSimpleConsole(opt =>
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
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true));
});
//services.AddHealthChecks().AddCheck("sql" ) //TODO: explore this
services.Configure<ApiBehaviorOptions>(opt =>
{
    opt.InvalidModelStateResponseFactory = ErrorResponseFactory.CreateErrorResult;
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
    if (isDevelopment)
    {
        // Allow all origins in development
        opt.AddDefaultPolicy(builder =>
        {
            builder.AllowAnyOrigin();
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
        });
    }
});

// ########################################
// ######## BUILD APP #####################
// ########################################

WebApplication app = builder.Build();

// ########################################
// ######## SET UP DATABASE ###############
// ########################################

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider serviceProvider = scope.ServiceProvider;
    ZapMeContext context = serviceProvider.GetRequiredService<ZapMeContext>();
    await context.Database.EnsureCreatedAsync();
    await context.Database.MigrateAsync();
    DataSeeders.Seed(context);
}

// ########################################
// ######## MIDDLEWARE PIPELINE ###########
// ########################################

if (isDevelopment)
{
    app.UseDeveloperExceptionPage();
    app.UseCors(); // Use default policy
}
else
{
    app.UseExceptionHandler("/error");
}

app.Map("/api", true, app =>
{
    // App!.UseHealthChecks("/api/v1/health/"); // TODO: explore this

    app.UseRouting();
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
    });
});

// ########################################
// ######## RUN APP #######################
// ########################################

app.Run();