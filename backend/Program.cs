using Amazon.S3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZapMe.Authentication;
using ZapMe.Constants;
using ZapMe.Data;
using ZapMe.Helpers;
using ZapMe.Middlewares;
using ZapMe.Options;
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

ZapMeOptions.Register(services, configuration);

services.AddAWSService<IAmazonS3>(configuration.GetAWSOptions("Cloudflare:R2"));
services.AddTransient<IImageManager, ImageManager>();
services.AddCloudflareTurnstileService(configuration);
services.AddDebounceService(configuration);
//services.AddGoogleReCaptchaService(configuration);
services.AddMailGunService(configuration);

services.AddTransient<IUserStore, UserStore>();
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
services.AddTransient<IEmailVerificationManager, EmailVerificationManager>();
services.AddTransient<IWebSocketInstanceManager, WebSocketInstanceManager>();

services.AddRateLimiting();
services.AddSwagger(isDevelopment);
services.AddAuthentication(ZapMeAuthenticationDefaults.AuthenticationScheme)
    .AddZapMe(configuration)
    .AddDiscord("discord", opt =>
    {
        opt.ClientId = configuration.GetValue<string>("Discord:OAuth2:ClientId")!;
        opt.ClientSecret = configuration.GetValue<string>("Discord:OAuth2:ClientSecret")!;
        opt.CallbackPath = "/api/v1/auth/o/cb/discord";
        opt.AccessDeniedPath = "/api/v1/auth/o/denied";
        opt.Scope.Add("email");
        opt.Scope.Add("identify");
        opt.Prompt = "none";
        //opt.CorrelationCookie.SameSite = SameSiteMode.None;
        opt.StateDataFormat = new DistributedCacheSecureDataFormat<AuthenticationProperties>();
        opt.Validate();
    })
    .AddGitHub("github", opt =>
    {
        opt.ClientId = configuration.GetValue<string>("GitHub:OAuth2:ClientId")!;
        opt.ClientSecret = configuration.GetValue<string>("GitHub:OAuth2:ClientSecret")!;
        opt.CallbackPath = "/api/v1/auth/o/cb/github";
        opt.AccessDeniedPath = "/api/v1/auth/o/denied";
        opt.Scope.Add("read:user");
        opt.Scope.Add("user:email");
        //opt.CorrelationCookie.SameSite = SameSiteMode.None;
        opt.StateDataFormat = new DistributedCacheSecureDataFormat<AuthenticationProperties>();
        opt.Validate();
    })
    .AddTwitter("twitter", opt =>
    {
        opt.ConsumerKey = configuration.GetValue<string>("Twitter:OAuth1:ConsumerKey")!;
        opt.ConsumerSecret = configuration.GetValue<string>("Twitter:OAuth1:ConsumerSecret")!;
        opt.CallbackPath = "/api/v1/auth/o/cb/twitter";
        opt.AccessDeniedPath = "/api/v1/auth/o/denied";
        opt.RetrieveUserDetails = true;
        //opt.StateCookie.SameSite = SameSiteMode.None;
        opt.StateDataFormat = new DistributedCacheSecureDataFormat<RequestToken>();
        opt.Validate();
    })
    .AddGoogle("google", opt =>
    {
        opt.ClientId = configuration.GetValue<string>("Google:OAuth2:ClientId")!;
        opt.ClientSecret = configuration.GetValue<string>("Google:OAuth2:ClientSecret")!;
        opt.CallbackPath = "/api/v1/auth/o/cb/google";
        opt.AccessDeniedPath = "/api/v1/auth/o/denied";
        opt.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
        opt.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
        opt.Scope.Add("openid");
        //opt.CorrelationCookie.SameSite = SameSiteMode.None;
        opt.StateDataFormat = new DistributedCacheSecureDataFormat<AuthenticationProperties>();
        opt.Validate();
    });
services.AddAuthorization(opt =>
{
    // Example:
    // opt.AddPolicy("Admin", policy => policy.RequireClaim("Admin"));
});
services.AddDatabase(configuration);
services.AddScheduledJobs();

// ########################################
// ######## CORS CONFIGURATION ############
// ########################################

services.AddCors(opt =>
{
    string[] defaultOrigins = configuration.GetSection("Cors:DefaultOrigins").Get<string[]>()!;

    opt.AddDefaultPolicy(builder => builder
        .WithOrigins(defaultOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
    );
    opt.AddPolicy("allow_oauth_providers", builder => builder
        .WithOrigins(
            "https://accounts.google.com",
            "https://oauth2.googleapis.com",
            "https://discord.com",
            "https://github.com",
            "https://api.twitter.com"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
    );
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
    app.UseCors();
    // Cloudflare caching: Asset is cached for 24 hours, and can be stale for 30 seconds while cloudflare revalidates it.
    app.UseHeaderValue("Cache-Control", "public, max-age=86400, stale-while-revalidate=30");
    app.UseStaticFiles();
});

// ########################################
// ######## RUN APP #######################
// ########################################

app.Run();