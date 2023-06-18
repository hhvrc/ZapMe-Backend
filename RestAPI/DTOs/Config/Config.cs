﻿namespace ZapMe.Controllers.Api.V1.Config.Models;

public sealed class Config
{
    /// <summary>
    /// Name of the product, e.g. "ZapMe"
    /// </summary>
    public required string AppName { get; set; }

    /// <summary>
    /// Version of the product, e.g. "1.0.0"
    /// </summary>
    public required string AppVersion { get; set; }

    /// <summary>
    /// The current Privacy Policy version, if the user has not accepted this version, they will be prompted to accept it
    /// </summary>
    public uint PrivacyPolicyVersion { get; set; }

    /// <summary>
    /// Markdown of the Privacy Policy
    /// </summary>
    public required string PrivacyPolicyMarkdown { get; set; }

    /// <summary>
    /// The current Terms of Service version, if the user has not accepted this version, they will be prompted to accept it
    /// </summary>
    public uint TermsOfServiceVersion { get; set; }

    /// <summary>
    /// Markdown of the Terms of Service
    /// </summary>
    public required string TermsOfServiceMarkdown { get; set; }

    /// <summary>
    /// Api configuration, e.g. version, etc.
    /// </summary>
    public ApiConfig Api { get; set; }

    /// <summary>
    /// Contact information for ZapMe, e.g. email address, discord server, etc.
    /// </summary>
    public ContactConfig Contact { get; set; }

    /// <summary>
    /// The social media accounts of the founder of ZapMe
    /// </summary>
    public SocialsConfig FounderSocials { get; set; }
}