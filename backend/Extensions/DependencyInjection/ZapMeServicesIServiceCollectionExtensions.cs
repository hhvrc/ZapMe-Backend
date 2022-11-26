using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using ZapMe;
using ZapMe.Services;
using ZapMe.Services.Interfaces;

namespace Microsoft.Extensions.DependencyInjection;

public static class ZapMeServicesIServiceCollectionExtensions
{
    public static void AddZapMeServices([NotNull] this IServiceCollection services)
    {
        services.AddZapMeHttpClients();

        services.AddZapMeUsers();
        services.AddZapMeSessions();
        services.AddZapMeLockOuts();
        services.AddZapMeUserRelations();
        services.AddZapMeFriendRequests();

        services.AddZapMeWebSockets();
    }

    public static void AddZapMeHttpClients([NotNull] this IServiceCollection services)
    {
        static void SetupHttpClient(HttpClient cli)
        {
            cli.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(Constants.AppName, Constants.AppVersion.String));
        }

        services.AddHttpClient<IDebounceService, DebounceService>(SetupHttpClient);
        services.AddHttpClient<IGoogleReCaptchaService, GoogleReCaptchaService>(SetupHttpClient);
        services.AddHttpClient<IMailGunService, MailGunService>(SetupHttpClient);
    }

    public static void AddZapMeUsers([NotNull] this IServiceCollection services)
    {
        services.AddTransient<IUserStore, UserStore>();
        services.AddTransient<IUserManager, UserManager>();
    }

    public static void AddZapMeSessions([NotNull] this IServiceCollection services)
    {
        services.AddTransient<ISessionStore, SessionStore>();
        services.AddTransient<ISessionManager, SessionManager>();
    }

    public static void AddZapMeLockOuts([NotNull] this IServiceCollection services)
    {
        services.AddTransient<ILockOutStore, LockOutStore>();
        services.AddTransient<ILockOutManager, LockOutManager>();
    }

    public static void AddZapMeUserRelations([NotNull] this IServiceCollection services)
    {
        services.AddTransient<IUserRelationStore, UserRelationStore>();
        services.AddTransient<IUserRelationManager, UserRelationManager>();
    }

    public static void AddZapMeFriendRequests([NotNull] this IServiceCollection services)
    {
        services.AddTransient<IFriendRequestStore, FriendRequestStore>();
        //services.AddTransient<IFriendRequestManager, FriendRequestManager>();
    }

    public static void AddZapMeWebSockets([NotNull] this IServiceCollection services)
    {
        services.AddTransient<IWebSocketInstanceManager, WebSocketInstanceManager>();
    }
}
