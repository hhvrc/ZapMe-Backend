using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Config.Models;

public class Config
{
    /// <summary>
    /// Name of the product, e.g. "ZapMe"
    /// </summary>
    [JsonPropertyName("name")]
    public required string ProductName { get; set; }

    [JsonPropertyName("app")]
    public AppConfig App { get; set; }

    [JsonPropertyName("api")]
    public ApiConfig Api { get; set; }

    [JsonPropertyName("contact")]
    public ContactConfig Contact { get; set; }

    [JsonPropertyName("community")]
    public CommunityConfig Community { get; set; }
}