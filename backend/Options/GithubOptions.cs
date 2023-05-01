namespace ZapMe.Options;

public sealed class GithubOptions
{
    public static string SectionName = "Github";

    public required GithubOAuth2Options OAuth2 { get; set; }
}

public sealed class GithubOAuth2Options
{
    public static string SectionName = GithubOptions.SectionName + ":OAuth2";

    public required string ClientID { get; set; }
    public required string ClientSecret { get; set; }
    public required string RedirectUri { get; set; }
}
