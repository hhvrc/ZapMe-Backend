using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Account.Models;

public readonly struct RecoveryRequestOk
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }
}
