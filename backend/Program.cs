using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using ZapMe.Authentication;
using ZapMe.Controllers;
using ZapMe.Services;
using ZapMe.Services.Interfaces;

// Set global Regex timeout to 1 second
AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(1000));

var builder = WebApplication.CreateBuilder(args);

var svc = builder.Services;
var cnf = builder.Configuration;

builder.Logging
    .AddSimpleConsole(cnf =>
    {
        cnf.IncludeScopes = true;
        cnf.SingleLine = false;
        cnf.UseUtcTimestamp = true;
        cnf.TimestampFormat = "[yyyy-MM-dd HH:mm:ss.fff] ";
    });


svc.AddRouting();
svc.AddControllers().AddJsonOptions(static opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true)));
svc.AddResponseCaching();
svc.AddMemoryCache();

svc.Configure<ApiBehaviorOptions>(static opt => opt.InvalidModelStateResponseFactory = ErrorResponseFactory.CreateErrorResult);

Action<HttpClient> SetupHttpClient = static cli =>
{
    cli.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("ZapMe", "1.0.0"));
};

svc.AddHttpClient<IDebounceService, DebounceService>(SetupHttpClient);
svc.AddHttpClient<IGoogleReCaptchaService, GoogleReCaptchaService>(SetupHttpClient);
svc.AddHttpClient<IMailGunService, MailGunService>(SetupHttpClient);

svc.AddTransient<IHybridCache, HybridCache>();
svc.AddTransient<IPasswordHasher, PasswordHasher>();
svc.AddTransient<IUserStore, UserStore>();
svc.AddTransient<IUserManager, UserManager>();
svc.AddTransient<ISignInStore, SignInStore>();
svc.AddTransient<ILockOutStore, LockOutStore>();
svc.AddTransient<ILockOutManager, LockOutManager>();
svc.AddTransient<ISignInManager, SignInManager>();
svc.AddTransient<IUserRelationStore, UserRelationStore>();
svc.AddTransient<IFriendRequestStore, FriendRequestStore>();
svc.AddTransient<IUserRelationManager, UserRelationManager>();
svc.AddTransient<IWebSocketInstanceManager, WebSocketInstanceManager>();

//TODO: explore this
//svc.AddHealthChecks().AddCheck("sql" )

svc.AddRateLimiter(opt =>
{
    opt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
    {
        ZapMe.Data.Models.SignInEntity? signIn = ctx.GetSignIn();
        if (signIn == null)
        {
            return RateLimitPartition.GetSlidingWindowLimiter(ctx.GetRemoteIP(), key => new SlidingWindowRateLimiterOptions()
            {
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                PermitLimit = 120,
                SegmentsPerWindow = 6,
                AutoReplenishment = true,
            });
        }

        ZapMe.Data.Models.UserEntity user = signIn.User;
        if (user?.UserRoles?.Select(r => r.RoleName).Contains("admin") ?? false)
        {
            return RateLimitPartition.GetNoLimiter("admin");
        }

        return RateLimitPartition.GetTokenBucketLimiter(signIn.UserId.ToString(), key => new TokenBucketRateLimiterOptions()
        {
            TokenLimit = 10,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 20,
            ReplenishmentPeriod = TimeSpan.FromSeconds(10),
            TokensPerPeriod = 5,
            AutoReplenishment = true
        });
    });
});

svc.AddEndpointsApiExplorer();
svc.AddSwaggerGen(opt =>
{
    opt.SchemaFilter<ZapMe.Attributes.ZapMeAttributeSwaggerFilter>();
    opt.ParameterFilter<ZapMe.Attributes.ZapMeAttributeSwaggerFilter>();

    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ZapMe",
        Description = "Open source application to control things",
        TermsOfService = new Uri("https://heavenvr.tech/tos"),
        Contact = new OpenApiContact
        {
            Name = "Contact me",
            Url = new Uri("https://heavenvr.tech/contact")
        },
        License = new OpenApiLicense
        {
            Name = "The Apache License, Version 2.0",
            Url = new Uri("https://heavenvr.tech/license")
        }
    });

    opt.SupportNonNullableReferenceTypes();

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


svc
    .AddAuthentication(ZapMeAuthenticationDefaults.AuthenticationScheme)
    .AddZapMe(opt =>
    {
        // CookieBuilder
        var cookie = opt.Cookie;
        cookie.Name = "cookie";
        cookie.HttpOnly = true;
        cookie.SameSite = SameSiteMode.Strict;
        cookie.SecurePolicy = CookieSecurePolicy.Always;
        cookie.IsEssential = true;

        // CookieAuthenticationOptions
        opt.ExpireTimeSpan = TimeSpan.FromDays(30);
        opt.SlidingExpiration = true;

        opt.Validate();
    })
    .AddGoogle(opt =>
    {
        opt.ClientId = cnf["Authorization:Google:ClientId"]!;
        opt.ClientSecret = cnf["Authorization:Google:ClientSecret"]!;
        opt.CallbackPath = cnf["Authorization:Google:CallbackPath"]!;
        opt.Scope.Add("openid");
        opt.Scope.Add(".../auth/userinfo.email");
        opt.Validate();
    })
    .AddGitHub(opt =>
    {
        opt.ClientId = cnf["Authorization:GitHub:ClientId"]!;
        opt.ClientSecret = cnf["Authorization:GitHub:ClientSecret"]!;
        opt.CallbackPath = cnf["Authorization:GitHub:CallbackPath"]!;
        opt.Scope.Add("user:email");
    })/*
    .AddTwitter(options => {
    })*/;
svc.AddAuthorization(opt =>
{
    // Example:
    // opt.AddPolicy("Admin", policy => policy.RequireClaim("Admin"));
});

svc.AddDbContextPool<ZapMe.Data.ZapMeContext>(opt =>
{
    opt.UseNpgsql(cnf["PgSQL:ConnectionString"], o => o.SetPostgresVersion(14, 5))
       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});
svc.AddStackExchangeRedisCache(opt =>
{
    opt.Configuration = cnf["Redis:ConnectionString"];
});

bool isDevelopment = builder.Environment.IsDevelopment();

if (isDevelopment)
{
    IdentityModelEventSource.ShowPII = true;
    svc.AddCors(opt =>
    {
        opt.AddPolicy("dev", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    });
}

var app = builder.Build();

if (isDevelopment)
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("dev");

app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.UseMiddleware<ZapMe.Middlewares.ActivityTracker>();

//TODO: explore this
//app.UseHealthChecks("/api/v1/health/");

//app.UseResponseCompression();

app.UseSwagger();
app.UseSwaggerUI(opt =>
{
    opt.SwaggerEndpoint("/swagger/v1/swagger.json", "ZapMe API - json");
    opt.SwaggerEndpoint("/swagger/v1/swagger.yaml", "ZapMe API - yaml");
});

// Finally endpoints
app.MapControllers();

app.Run();