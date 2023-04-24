using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Account.Models;

public readonly struct CreateOk
{
    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("message")]
    public required string Message { get; init; }
}
