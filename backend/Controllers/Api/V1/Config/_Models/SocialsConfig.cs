using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Config.Models;

public struct SocialsConfig
{
    /// <summary>
    /// Uri pointing to a github account
    /// </summary>
    [JsonPropertyName("githubUri")]
    public Uri? GithubUri { get; set; }

    /// <summary>
    /// Uri pointing to a twitter account
    /// </summary>
    [JsonPropertyName("twitterUri")]
    public Uri? TwitterUri { get; set; }

    /// <summary>
    /// Uri pointing to a reddit account
    /// </summary>
    [JsonPropertyName("redditUri")]
    public Uri? RedditUri { get; set; }

    /// <summary>
    /// Uri pointing to a personal website
    /// </summary>
    [JsonPropertyName("websiteUri")]
    public Uri? WebsiteUri { get; set; }
}
