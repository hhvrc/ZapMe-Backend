using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Config.Models;

public struct ApiDisabledFeatures
{
    /// <summary>
    /// If true, WebRTC is disabled
    /// </summary>
    [JsonPropertyName("webrtc")]
    public bool WebRTC { get; set; }

    /// <summary>
    /// If true, WebSockets are disabled
    /// </summary>
    [JsonPropertyName("websockets")]
    public bool WebSockets { get; set; }

    /// <summary>
    /// List of disabled endpoints, if an endpoint is in this list, it will return a "503 Service Unavailable"
    /// </summary>
    [JsonPropertyName("endpoints")]
    public string[] Endpoints { get; set; }
}