using System.Text.Json.Serialization;

namespace ZapMe.DTOs;

public readonly struct CloudflareTurnstileVerifyResponse
{
    [JsonPropertyName("success")]
    public readonly bool Success { get; init; }

    [JsonPropertyName("challenge_ts")]
    public readonly DateTime ChallengeTimeStamp { get; init; }

    [JsonPropertyName("hostname")]
    public readonly string? Hostname { get; init; }

    [JsonPropertyName("error-codes")]
    public readonly string[]? ErrorCodes { get; init; }

    [JsonPropertyName("action")]
    public readonly string? Action { get; init; }

    [JsonPropertyName("cdata")]
    public readonly string? CData { get; init; }
}