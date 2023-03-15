using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Config.Models;

public struct AppConfig
{
    /// <summary>
    /// The current version of the App, if the local version is lower than this, the user will be notified of an update
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } // TODO: make this a string
}