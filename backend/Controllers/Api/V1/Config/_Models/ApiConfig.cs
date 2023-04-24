namespace ZapMe.Controllers.Api.V1.Config.Models;

public readonly struct ApiConfig
{
    /// <summary>
    /// The current Terms of Service version, if the user has not accepted this version, they will be prompted to accept it
    /// </summary>
    public int TosVersion { get; init; }

    /// <summary>
    /// The current Privacy Policy version, if the user has not accepted this version, they will be prompted to accept it
    /// </summary>
    public int PrivacyVersion { get; init; }

    /// <summary>
    /// The DSN for Sentry, used for error reporting
    /// If this is null, Sentry reporting is disabled
    /// </summary>
    public Uri? SentryDsn { get; init; }

    /// <summary>
    /// Trace sample rate for Sentry, used for performance monitoring
    /// </summary>
    public double SentryTraceSampleRate { get; init; }

    public AuthenticationConfig Authentication { get; init; }

    public ApiDisabledFeatures DisabledFeatures { get; init; }
}