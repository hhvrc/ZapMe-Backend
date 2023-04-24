namespace ZapMe.Controllers.Api.V1.Config.Models;

public readonly struct ApiDisabledFeatures
{
    /// <summary>
    /// If true, WebRTC is disabled
    /// </summary>
    public bool WebRTC { get; init; }

    /// <summary>
    /// If true, WebSockets are disabled
    /// </summary>
    public bool WebSockets { get; init; }

    /// <summary>
    /// List of disabled endpoints, if an endpoint is in this list, it will return a "503 Service Unavailable"
    /// </summary>
    public string[] Endpoints { get; init; }
}