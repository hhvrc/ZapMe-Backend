using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using ZapMe.Constants;
using ZapMe.Services;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace Microsoft.Extensions.DependencyInjection;

public static class ZapMeIServiceCollectionExtensions
{
    public static void ZMAddHttpClients([NotNull] this IServiceCollection services, [NotNull] IConfiguration config)
    {
        services.AddHttpClient("Debounce", cli =>
        {
            cli.BaseAddress = new Uri("https://disposable.debounce.io/", UriKind.Absolute);
            cli.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Application.Json));
            cli.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(App.AppName, App.AppVersion.String));
        });
        services.AddHttpClient("GoogleReCaptcha", cli =>
        {
            cli.BaseAddress = new Uri("https://www.google.com/recaptcha/api/", UriKind.Absolute);
            cli.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Application.Json));
            cli.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(App.AppName, App.AppVersion.String));
        });
        services.AddHttpClient("CloudflareTurnstile", cli =>
        {
            cli.BaseAddress = new Uri("https://challenges.cloudflare.com/turnstile/v0/", UriKind.Absolute);
            cli.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Application.Json));
            cli.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(App.AppName, App.AppVersion.String));
        });
        services.AddHttpClient("MailGun", cli =>
        {
            string apiKey = config.GetOrThrow("Mailgun:ApiKey");

            cli.BaseAddress = new Uri("https://api.mailgun.net/v3/", UriKind.Absolute);
            cli.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Application.Json));
            cli.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(App.AppName, App.AppVersion.String));
            cli.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"api:{apiKey}")));
        });

        services.AddTransient<IDebounceService, DebounceService>();
        //services.AddTransient<IGoogleReCaptchaService, GoogleReCaptchaService>();
        services.AddTransient<ICloudFlareTurnstileService, CloudFlareTurnstileService>();
        services.AddTransient<IMailGunService, MailGunService>();
    }

    public static void ZMAddUsers([NotNull] this IServiceCollection services)
    {
        services.AddTransient<IUserManager, UserManager>();
        services.AddTransient<IPasswordResetRequestStore, PasswordResetRequestStore>();
        services.AddTransient<IPasswordResetManager, PasswordResetManager>();
        services.AddTransient<IImageManager, ImageManager>();
    }

    public static void ZMAddSessions([NotNull] this IServiceCollection services)
    {
        services.AddTransient<IUserAgentStore, UserAgentStore>();
        services.AddTransient<IUserAgentManager, UserAgentManager>();
        services.AddTransient<ISessionStore, SessionStore>();
        services.AddTransient<ISessionManager, SessionManager>();
    }

    public static void ZMAddLockOuts([NotNull] this IServiceCollection services)
    {
        services.AddTransient<ILockOutStore, LockOutStore>();
    }

    public static void ZMAddUserRelations([NotNull] this IServiceCollection services)
    {
        services.AddTransient<IUserRelationStore, UserRelationStore>();
    }

    public static void ZMAddFriendRequests([NotNull] this IServiceCollection services)
    {
        services.AddTransient<IFriendRequestStore, FriendRequestStore>();
        //services.AddTransient<IFriendRequestManager, FriendRequestManager>();
    }

    public static void ZMAddEmailTemplates([NotNull] this IServiceCollection services)
    {
        //services.AddTransient<EmailTemplateStore>();
        //services.AddTransient<IEmailTemplateStore, CachedEmailTemplateStore>();
        services.AddTransient<IEmailTemplateStore, EmailTemplateStore>();
    }

    public static void ZMAddWebSockets([NotNull] this IServiceCollection services)
    {
        services.AddTransient<IWebSocketInstanceManager, WebSocketInstanceManager>();
    }
}
