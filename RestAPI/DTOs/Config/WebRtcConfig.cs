namespace ZapMe.DTOs.Config;

public readonly struct WebRtcConfig
{
    public bool Enabled { get; init; }
    public string[] ApprovedStunServers { get; init; }
}