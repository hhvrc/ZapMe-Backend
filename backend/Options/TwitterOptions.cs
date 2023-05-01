namespace ZapMe.Options;

public sealed class TwitterOptions
{
    public const string SectionName = "Twitter";

    public required TwitterOAuth2Options OAuth2 { get; set; }
}

public sealed class TwitterOAuth2Options
{
    public const string SectionName = TwitterOptions.SectionName + ":OAuth2";

    public required string ClientID { get; set; }
    public required string ClientSecret { get; set; }
}
