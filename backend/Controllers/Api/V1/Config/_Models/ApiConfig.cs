using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Config.Models;

public struct ApiConfig
{
    [JsonPropertyName("version")]
    public Version Version { get; set; } // TODO: make this a string

    [JsonPropertyName("tos_version")]
    public int TosVersion { get; set; }

    [JsonPropertyName("disabled_features")]
    public ApiDisabledFeatures DisabledFeatures { get; set; }
}