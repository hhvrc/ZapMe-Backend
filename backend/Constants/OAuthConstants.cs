namespace ZapMe.Constants;

public static class OAuthConstants
{
    public const string DiscordProviderName = "discord";
    public const string GitHubProviderName = "github";
    public const string TwitterProviderName = "twitter";
    public const string GoogleProviderName = "google";

    public static readonly TimeSpan StateLifetime = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan RegistrationTicketLifetime = TimeSpan.FromMinutes(15);
}
