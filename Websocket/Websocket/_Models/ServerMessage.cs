using System.Text.Json.Serialization;

namespace ZapMe.Websocket.Models;

public sealed class ServerMessage<T>
{
    public ServerMessage(ServerMessageType messageType, T data)
    {
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        MessageType = messageType;
        Data = data;
    }

    [JsonPropertyName("t")]
    public long Timestamp { get; set; }

    [JsonPropertyName("m")]
    public ServerMessageType MessageType { get; set; }

    [JsonPropertyName("d")]
    public T Data { get; set; }
}
