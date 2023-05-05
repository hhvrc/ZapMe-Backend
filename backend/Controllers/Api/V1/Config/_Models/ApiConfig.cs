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

    public ApiDisabledFeatures DisabledFeatures { get; init; }
}