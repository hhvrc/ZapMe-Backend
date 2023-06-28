using System.Text.Json.Serialization;

namespace ZapMe.DTOs;

public struct GoogleReCaptchaVerifyResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("challenge_ts")]
    public DateTime ChallengeTimeStamp { get; set; }

    [JsonPropertyName("hostname")]
    public string? Hostname { get; set; }

    [JsonPropertyName("apk_package_name")]
    public string? ApkPackageName { get; set; }

    [JsonPropertyName("error-codes")]
    public string[]? ErrorCodes { get; set; }
}