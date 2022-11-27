using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using ZapMe;
using ZapMe.Services;
using ZapMe.Services.Interfaces;

namespace Microsoft.Extensions.DependencyInjection;

public static class ZapMeIServiceCollectionExtensions
{
    public static void ZMAddHttpClients([NotNull] this IServiceCollection services)
    {
        static void SetupHttpClient(HttpClient cli)
        {
            cli.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(Constants.AppName, Constants.AppVersion.String));
        }

        services.AddHttpClient<IDebounceService, DebounceService>(SetupHttpClient);
        services.AddHttpClient<IGoogleReCaptchaService, GoogleReCaptchaService>(SetupHttpClient);
        services.AddHttpClient<IMailGunService, MailGunService>(SetupHttpClient);
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
