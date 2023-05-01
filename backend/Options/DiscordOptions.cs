namespace ZapMe.Options;

public sealed class DiscordOptions
{
    public const string SectionName = "Discord";

    public required DiscordOAuth2Options OAuth2 { get; set; }
}

public sealed class DiscordOAuth2Options
{
    public const string SectionName = DiscordOptions.SectionName + ":OAuth2";

    public required string ClientID { get; set; }
    public required string ClientSecret { get; set; }
    public required string RedirectUri { get; set; }
    public required string[] Scopes { get; set; }
}
