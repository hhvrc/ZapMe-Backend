using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Config.Models;

public struct ApiConfig
{
    /// <summary>
    /// The current Terms of Service version, if the user has not accepted this version, they will be prompted to accept it
    /// </summary>
    [JsonPropertyName("tos_version")]
    public int TosVersion { get; set; }

    /// <summary>
    /// The current Privacy Policy version, if the user has not accepted this version, they will be prompted to accept it
    /// </summary>
    [JsonPropertyName("privacy_version")]
    public int PrivacyVersion { get; set; }

    /// <summary>
    /// The DSN for Sentry, used for error reporting
    /// If this is null, Sentry reporting is disabled
    /// </summary>
    [JsonPropertyName("sentry_dsn")]
    public Uri? SentryDsn { get; set; }

    /// <summary>
    /// Trace sample rate for Sentry, used for performance monitoring
    /// </summary>
    [JsonPropertyName("sentry_trace_sample_rate")]
    public double SentryTraceSampleRate { get; set; }

    [JsonPropertyName("authentication")]
    public AuthenticationConfig Authentication { get; set; }

    [JsonPropertyName("disabled_features")]
    public ApiDisabledFeatures DisabledFeatures { get; set; }
}