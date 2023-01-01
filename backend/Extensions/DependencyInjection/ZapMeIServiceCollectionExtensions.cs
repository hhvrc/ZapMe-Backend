using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ZapMe;
using ZapMe.Controllers.Api.V1.Config.Models;
using ZapMe.Services;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace Microsoft.Extensions.DependencyInjection;

public static class ZapMeIServiceCollectionExtensions
{
    public static void ZMAddHttpClients([NotNull] this IServiceCollection services, [NotNull] IConfiguration configuration)
    {
        services.AddHttpClient("Debounce", static config =>
        {
            config.BaseAddress = new Uri($"https://disposable.debounce.io/", UriKind.Absolute);
            config.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Application.Json));
            config.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue(Encoding.UTF8.WebName));
            config.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(Constants.AppName, Constants.AppVersion.String));
        });
        services.AddHttpClient("GoogleReCaptcha", static config =>
        {
            config.BaseAddress = new Uri("https://www.google.com/recaptcha/api/", UriKind.Absolute);
            config.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Application.Json));
            config.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue(Encoding.UTF8.WebName));
            config.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(Constants.AppName, Constants.AppVersion.String));
        });
        services.AddHttpClient("MailGun", config =>
        {
            config.BaseAddress = new Uri("https://api.eu.mailgun.net/v3/", UriKind.Absolute);
            config.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Application.Json));
            config.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue(Encoding.UTF8.WebName));
            config.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(Constants.AppName, Constants.AppVersion.String));
            config.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"api:{configuration["Mailgun:ApiKey"]}")));
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
