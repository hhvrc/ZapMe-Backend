using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Config.Models;

public readonly struct ApiConfig
{
    /// <summary>
    /// The current Terms of Service version, if the user has not accepted this version, they will be prompted to accept it
    /// </summary>
    [JsonPropertyName("tos_version")]
    public int TosVersion { get; init; }

    /// <summary>
    /// The current Privacy Policy version, if the user has not accepted this version, they will be prompted to accept it
    /// </summary>
    [JsonPropertyName("privacy_version")]
    public int PrivacyVersion { get; init; }

    /// <summary>
    /// The DSN for Sentry, used for error reporting
    /// If this is null, Sentry reporting is disabled
    /// </summary>
    [JsonPropertyName("sentry_dsn")]
    public Uri? SentryDsn { get; init; }

    /// <summary>
    /// Trace sample rate for Sentry, used for performance monitoring
    /// </summary>
    [JsonPropertyName("sentry_trace_sample_rate")]
    public double SentryTraceSampleRate { get; init; }

    [JsonPropertyName("authentication")]
    public AuthenticationConfig Authentication { get; init; }

    [JsonPropertyName("disabled_features")]
    public ApiDisabledFeatures DisabledFeatures { get; init; }
}