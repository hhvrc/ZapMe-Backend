using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Config.Models;

public struct AppConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}