using System.Text.Json.Serialization;
using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Config.Models;

public readonly struct ContactConfig
{
    /// <summary>
    /// Email address to contact the support of the service
    /// </summary>
    [EmailAddress]
    public string EmailSupport { get; init; }

    /// <summary>
    /// Email address to contact the owner of the service
    /// </summary>
    [EmailAddress]
    public string EmailContact { get; init; }

    /// <summary>
    /// Invite URL to the Discord server where users can get support
    /// </summary>
    public Uri DiscordInviteUrl { get; init; }
}