using System.Text.Json.Serialization;

namespace ZapMe.DTOs;

public readonly struct GoogleReCaptchaVerifyResponse
{
    [JsonPropertyName("success")]
    public readonly bool Success { get; init; }

    [JsonPropertyName("challenge_ts")]
    public readonly DateTime ChallengeTimeStamp { get; init; }

    [JsonPropertyName("hostname")]
    public readonly string? Hostname { get; init; }

    [JsonPropertyName("apk_package_name")]
    public readonly string? ApkPackageName { get; init; }

    [JsonPropertyName("error-codes")]
    public readonly string[]? ErrorCodes { get; init; }
}