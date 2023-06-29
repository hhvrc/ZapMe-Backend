using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZapMe.Constants;
using ZapMe.Database.Extensions;
using ZapMe.Extensions;
using ZapMe.Middlewares;
using ZapMe.Options;
using ZapMe.Options.OAuth;
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
builder.Logging.AddSimpleConsole();

// ########################################
// ######## CORE SERVICES #################
// ########################################

builder.Services.Configure<ForwardedHeadersOptions>(opt =>
{
    opt.ForwardedHeaders = ForwardedHeaders.All;

    opt.KnownNetworks.Clear();
    foreach (string ip in configuration.GetSection("Proxy:SubNetList").Get<string[]>() ?? Array.Empty<string>())
    {
        string[] parts = ip.Split('/');
        if (parts.Length != 2 || !IPAddress.TryParse(parts[0], out IPAddress? address) || !Int32.TryParse(parts[1], out int mask) || mask < 0 || mask > 32)
        {
            Console.WriteLine($"Skipping invalid proxy subnet: {ip}");
            continue;
        }

        opt.KnownNetworks.Add(new IPNetwork(address, mask));
    }

    opt.KnownProxies.Clear();
    foreach (string ip in configuration.GetSection("Proxy:AllowedIPs").Get<string[]>() ?? Array.Empty<string>())
    {
        if (!IPAddress.TryParse(ip, out IPAddress? address))
        {
            Console.WriteLine($"Skipping invalid proxy ip: {ip}");
            continue;
        }
        opt.KnownProxies.Add(address);
    }
});
services.AddRouting(opt => opt.LowercaseUrls = true);
services.AddResponseCaching();
services.AddControllers(opt =>
{
    opt.CacheProfiles.Add("no-store", new CacheProfile { Duration = 0, Location = ResponseCacheLocation.None, NoStore = true });
})
.AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
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

JwtOptions.Register(services, configuration);
DiscordBotOptions.Register(services, configuration);
DiscordOAuth2Options.Register(services, configuration);
GitHubOAuth2Options.Register(services, configuration);
GoogleOAuth2Options.Register(services, configuration);
TwitterOAuth1Options.Register(services, configuration);
CloudflareTurnstileOptions.Register(services, configuration);
GoogleReCaptchaOptions.Register(services, configuration);
MailGunOptions.Register(services, configuration);

services.AddCloudflareR2(configuration);
services.AddTransient<IImageManager, ImageManager>();
services.AddCloudflareTurnstileService(configuration);
services.AddDebounceService(configuration);
//services.AddGoogleReCaptchaService(configuration);
services.AddMailGunService(configuration);

services.AddTransient<IJwtAuthenticationManager, JwtAuthenticationManager>();
services.AddTransient<IPasswordResetRequestStore, PasswordResetRequestStore>();
services.AddTransient<IPasswordResetManager, PasswordResetManager>();
services.AddTransient<IUserRelationManager, UserRelationManager>();
services.AddTransient<IUserAgentStore, UserAgentStore>();
services.AddTransient<IUserAgentManager, UserAgentManager>();
services.AddTransient<ISessionStore, SessionStore>();
services.AddTransient<ISessionManager, SessionManager>();
services.AddTransient<ILockOutStore, LockOutStore>();
//services.AddTransient<IFriendRequestManager, FriendRequestManager>();
services.AddTransient<IEmailVerificationManager, EmailVerificationManager>();
services.AddTransient<ISSOStateStore, SSOStateStore>();
services.AddSingleton<IDiscordBotService, DiscordBotService>();
services.AddSingleton<IWebSocketInstanceManager, WebSocketInstanceManager>();

services.AddRateLimiting();
services.AddSwagger(isDevelopment);
services.AddAuthentication(AuthenticationConstants.ZapMeScheme)
    .AddZapMe()
    .AddOAuthProviders(configuration);
services.AddAuthorization(opt =>
{
    // Example:
    // opt.AddPolicy("Admin", policy => policy.RequireClaim("Admin"));
});
services.AddStackExchangeRedisCache(opt =>
{
    opt.Configuration = configuration.GetValue<string>("Redis:ConnectionString")!;
    opt.InstanceName = App.AppName;
});
services.AddZapMeDatabase(configuration);
services.AddScheduledJobs();
services.AddMediator();

// ########################################
// ######## CORS CONFIGURATION ############
// ########################################

services.AddCors(opt =>
{
    string[] defaultOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()!;

    opt.AddDefaultPolicy(builder => builder
        .WithOrigins(defaultOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
    );
    opt.AddPolicy("allow_oauth_providers", builder => builder
        .WithOrigins(
            "https://discord.com",
            "https://*.discord.com",
            "https://github.com",
            "https://*.github.com",
            "https://twitter.com",
            "https://*.twitter.com",
            "https://google.com",
            "https://*.google.com",
            "https://googleapis.com",
            "https://*.googleapis.com"
        )
        .AllowAnyHeader()
        .WithMethods(
            "GET",
            "POST",
            "OPTIONS"
        )
        .AllowCredentials()
    );
});

// ########################################
// ######## BUILD APP #####################
// ########################################

WebApplication app = builder.Build();

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

app.UseForwardedHeaders();

app.Map("/api", true, app =>
{
    // App!.UseHealthChecks("/api/v1/health/"); // TODO: explore this

    app.UseRouting();
    app.UseCors(); // Use default policy
    app.UseResponseCaching();
    app.UseAuthorization();
    app.UseRateLimiter();
    app.UseMiddleware<ActivityTracker>();
    app.UseWebSockets();
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
    app.UseCors();
    // Cloudflare caching: Asset is cached for 24 hours, and can be stale for 30 seconds while cloudflare revalidates it.
    app.UseHeaderValue("Cache-Control", "public, max-age=86400, stale-while-revalidate=30");
    app.UseStaticFiles();
});

// ########################################
// ######## RUN APP #######################
// ########################################

app.Run();