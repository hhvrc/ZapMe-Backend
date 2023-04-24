using System.Text.Json.Serialization;

namespace ZapMe.Websocket.Models.ServerPayloads;

public sealed class ReadyMessage
{
    [JsonPropertyName("heartbeat_interval")]
    public uint HeartbeatIntervalMs { get; set; }
}
