using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Config.Models;

public struct CommunityConfig
{
    [JsonPropertyName("discord_invite_url")]
    public string DiscordInviteUrl { get; set; }
}