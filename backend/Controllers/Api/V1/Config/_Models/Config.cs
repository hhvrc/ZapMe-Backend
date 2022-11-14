using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Config.Models;

public struct Config
{
    [JsonPropertyName("app")]
    public AppConfig App { get; set; }

    [JsonPropertyName("api")]
    public ApiConfig Api { get; set; }

    [JsonPropertyName("email")]
    public Emailconfig Email { get; set; }

    [JsonPropertyName("community")]
    public CommunityConfig Community { get; set; }
}