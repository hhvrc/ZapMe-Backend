using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZapMe.Constants;
using ZapMe.Controllers.Api.V1.Config.Models;
using ZapMe.Data;
using ZapMe.Middlewares;
using ZapMe.Services;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

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
    opt.InvalidModelStateResponseFactory = ActionContextUtils.CreateErrorResult;
});

// ########################################
// ######## ZAPME SERVICES ################
// ########################################

services.AddAWSService<IAmazonS3>(configuration.GetAWSOptions("Cloudflare:R2"));
services.AddTransient<IImageManager, ImageManager>();
services.AddCloudflareTurnstileService(configuration);
services.AddDebounceService(configuration);
//services.AddGoogleReCaptchaService(configuration);
services.AddMailGunService(configuration);

services.AddTransient<IUserManager, UserManager>();
services.AddTransient<IPasswordResetRequestStore, PasswordResetRequestStore>();
services.AddTransient<IPasswordResetManager, PasswordResetManager>();
services.AddTransient<IUserAgentStore, UserAgentStore>();
services.AddTransient<IUserAgentManager, UserAgentManager>();
services.AddTransient<ISessionStore, SessionStore>();
services.AddTransient<ISessionManager, SessionManager>();
services.AddTransient<ILockOutStore, LockOutStore>();
services.AddTransient<IUserRelationStore, UserRelationStore>();
services.AddTransient<IFriendRequestStore, FriendRequestStore>();
//services.AddTransient<IFriendRequestManager, FriendRequestManager>();
services.AddTransient<IEmailTemplateStore, EmailTemplateStore>();
services.AddTransient<IEmailVerificationManager, EmailVerificationManager>();
services.AddTransient<IWebSocketInstanceManager, WebSocketInstanceManager>();

services.ZMAddRateLimiter();
services.ZMAddSwagger(isDevelopment);
services.ZMAddAuthentication(configuration);
services.ZMAddAuthorization();
services.ZMAddDatabase(configuration);
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
    using IServiceScope scope = app.Services.CreateScope();

    ZapMeContext context = scope.ServiceProvider.GetRequiredService<ZapMeContext>();

    if (context.Database.EnsureCreated())
    {
        await context.Database.MigrateAsync();
        await DataSeeders.SeedAsync(context);
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
    // Cloudflare caching: Asset is cached for 10 minutes, and can be stale for 30 seconds while cloudflare revalidates it.
    app.UseHeaderValue("Cache-Control", "public, max-age=600, stale-while-revalidate=30");
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
    // Cloudflare caching: Asset is cached for 24 hours, and can be stale for 30 seconds while cloudflare revalidates it.
    app.UseHeaderValue("Cache-Control", "public, max-age=86400, stale-while-revalidate=30");
    app.UseStaticFiles();
});

// ########################################
// ######## RUN APP #######################
// ########################################

app.Run();