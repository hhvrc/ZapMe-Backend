using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using ZapMe;
using ZapMe.Services;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace Microsoft.Extensions.DependencyInjection;

public static class ZapMeIServiceCollectionExtensions
{
    private static void AddHttpClientConsumingJson(IServiceCollection services, string name, string baseUri, Action<HttpClient>? configureClient = null)
    {
        services.AddHttpClient(name, config =>
        {
            config.BaseAddress = new Uri(baseUri, UriKind.Absolute);
            config.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Application.Json));
            config.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue(Encoding.UTF8.WebName));
            config.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(Constants.AppName, Constants.AppVersion.String));
            configureClient?.Invoke(config);
        });
    }

    public static void ZMAddHttpClients([NotNull] this IServiceCollection services, [NotNull] IConfiguration configuration)
    {
        AddHttpClientConsumingJson(services, "Debounce", "https://disposable.debounce.io/");
        AddHttpClientConsumingJson(services, "ReCaptcha", "https://www.google.com/recaptcha/api/");
        AddHttpClientConsumingJson(services, "MailGun", "https://api.eu.mailgun.net/v3/", config =>
        {
            string apiKey = configuration["MailGun:ApiKey"] ?? throw new KeyNotFoundException("Could not find \"MailGun:ApiKey\" in configuration");

            string encodedApiKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("api:" + apiKey));
            
            config.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedApiKey);
        });

        services.AddScoped<IDebounceService, DebounceService>();
        services.AddScoped<IGoogleReCaptchaService, GoogleReCaptchaService>();
        services.AddScoped<IMailGunService, MailGunService>();
    }

    public static void ZMAddPasswordHashing([NotNull] this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
    }

    public static void ZMAddUsers([NotNull] this IServiceCollection services)
    {
        services.AddTransient<IUserStore, UserStore>();
        services.AddTransient<IUserManager, UserManager>();
    }

    public static void ZMAddSessions([NotNull] this IServiceCollection services)
    {
        services.AddTransient<ISessionStore, SessionStore>();
        services.AddTransient<ISessionManager, SessionManager>();
    }

    public static void ZMAddLockOuts([NotNull] this IServiceCollection services)
    {
        services.AddTransient<ILockOutStore, LockOutStore>();
        services.AddTransient<ILockOutManager, LockOutManager>();
    }

    public static void ZMAddUserRelations([NotNull] this IServiceCollection services)
    {
        services.AddTransient<IUserRelationStore, UserRelationStore>();
        services.AddTransient<IUserRelationManager, UserRelationManager>();
    }

    public static void ZMAddFriendRequests([NotNull] this IServiceCollection services)
    {
        services.AddTransient<IFriendRequestStore, FriendRequestStore>();
        //services.AddTransient<IFriendRequestManager, FriendRequestManager>();
    }

    public static void ZMAddWebSockets([NotNull] this IServiceCollection services)
    {
        services.AddTransient<IWebSocketInstanceManager, WebSocketInstanceManager>();
    }
}
