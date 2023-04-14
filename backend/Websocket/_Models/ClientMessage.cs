using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ZapMe.Websocket.Models;

public sealed class ClientMessage
{
    [JsonPropertyName("t")]
    public ulong Timestamp { get; set; }

    [JsonPropertyName("m")]
    public ClientMessageType MessageType { get; set; }

    [JsonPropertyName("d")]
    public JsonObject? Data { get; set; }
}
