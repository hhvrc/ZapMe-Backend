namespace ZapMe.Constants;

public static class AuthenticationConstants
{
    public const string JwtIssuer = App.AppName;
    public const string JwtAudience = App.AppName;
    public const string ZapMePrefix = App.AppName + ":";

    public const string ZapMeScheme = App.AppName;
    public const string DiscordScheme = "discord";
    public const string GitHubScheme = "github";
    public const string TwitterScheme = "twitter";
    public const string GoogleScheme = "google";
    public static readonly string[] OAuth2Schemes = { DiscordScheme, GitHubScheme, TwitterScheme, GoogleScheme };

    public const string ContextKey_User = ZapMePrefix + "user";
    public const string ContextKey_Session = ZapMePrefix + "session";
}
