using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Config.Models;

public struct CommunityConfig
{
    /// <summary>
    /// Invite uri to the community discord server
    /// </summary>
    [JsonPropertyName("discord_invite_uri")]
    public Uri DiscordInviteUri { get; set; }

    /// <summary>
    /// Social media links to project founder's accounts
    /// </summary>
    [JsonPropertyName("socials")]
    public SocialEntry[] Socials { get; set; }
}
