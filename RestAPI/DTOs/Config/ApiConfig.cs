namespace ZapMe.DTOs.Config;

public readonly struct ApiConfig
{
    /// <summary>
    /// Name of the product, e.g. "ZapMe"
    /// </summary>
    public required string AppName { get; init; }

    /// <summary>
    /// Version of the product, e.g. "1.0.0"
    /// </summary>
    public required string AppVersion { get; init; }

    /// <summary>
    /// The current Privacy Policy version, if the user has not accepted this version, they will be prompted to accept it
    /// </summary>
    public uint PrivacyPolicyVersion { get; init; }

    /// <summary>
    /// Markdown of the Privacy Policy
    /// </summary>
    public required string PrivacyPolicyMarkdown { get; init; }

    /// <summary>
    /// The current Terms of Service version, if the user has not accepted this version, they will be prompted to accept it
    /// </summary>
    public uint TermsOfServiceVersion { get; init; }

    /// <summary>
    /// Markdown of the Terms of Service
    /// </summary>
    public required string TermsOfServiceMarkdown { get; init; }

    /// <summary>
    /// WebRTC configuration
    /// </summary>
    public WebRtcConfig WebRtc { get; init; }

    /// <summary>
    /// Contact information for ZapMe, e.g. email address, discord server, etc.
    /// </summary>
    public ContactConfig Contact { get; init; }

    /// <summary>
    /// The social media accounts of the founder of ZapMe
    /// </summary>
    public SocialsConfig FounderSocials { get; init; }
}