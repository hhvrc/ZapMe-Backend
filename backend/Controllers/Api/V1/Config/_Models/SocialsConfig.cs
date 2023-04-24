using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Config.Models;

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
}
