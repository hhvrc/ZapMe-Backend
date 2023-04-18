﻿using System.Text.Json.Serialization;
using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Config.Models;

public struct ContactConfig
{
    /// <summary>
    /// Email address to contact the support of the service
    /// </summary>
    [EmailAddress]
    [JsonPropertyName("support")]
    public string EmailSupport { get; set; }

    /// <summary>
    /// Email address to contact the owner of the service
    /// </summary>
    [EmailAddress]
    [JsonPropertyName("contact")]
    public string EmailContact { get; set; }

    /// <summary>
    /// Invite URL to the Discord server where users can get support
    /// </summary>
    [JsonPropertyName("discord")]
    public Uri DiscordInviteUrl { get; set; }
}