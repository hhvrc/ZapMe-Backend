using System.Text.Json.Serialization;

namespace ZapMe.Websocket.Models.ServerPayloads;

public sealed class GlobalMessage
{
    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("message")]
    public required string Message { get; set; }
}
