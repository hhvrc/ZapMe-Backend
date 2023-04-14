using System.Text.Json.Serialization;

namespace ZapMe.Websocket._Models.ServerPayloads;

public sealed class ReadyMessage
{
    [JsonPropertyName("heartbeat_interval")]
    public uint HeartbeatIntervalMs { get; set; }
}
