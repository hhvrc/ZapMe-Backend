using System.Text.Json.Serialization;

namespace ZapMe.Websocket.Models.ServerPayloads;

public sealed class ReadyMessage
{
    /// <summary>
    /// 
    /// </summary>
    public uint HeartbeatIntervalMs { get; set; }
}
