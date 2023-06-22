namespace ZapMe.DTOs.Config;

public readonly struct SocialsConfig
{
    /// <summary>
    /// Uri pointing to a github account
    /// </summary>
    public Uri? GithubUri { get; init; }

    /// <summary>
    /// Uri pointing to a twitter account
    /// </summary>
    public Uri? TwitterUri { get; init; }

    /// <summary>
    /// Uri pointing to a reddit account
    /// </summary>
    public Uri? RedditUri { get; init; }

    /// <summary>
    /// Uri pointing to a personal website
    /// </summary>
    public Uri? WebsiteUri { get; init; }

    /// <summary>
    /// Discord username, e.g. "username#1234"
    /// </summary>
    public string? DiscordUsername { get; init; }
}
