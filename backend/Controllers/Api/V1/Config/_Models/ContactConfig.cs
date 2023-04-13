﻿using System.Text.Json.Serialization;
using ZapMe.Attributes;

namespace ZapMe.Controllers.Api.V1.Config.Models;

public struct ContactConfig
{
    [EmailAddress]
    [JsonPropertyName("support")]
    public string EmailSupport { get; set; }

    [EmailAddress]
    [JsonPropertyName("contact")]
    public string EmailContact { get; set; }
}