namespace ZapMe.Controllers.Api.V1.Config.Models;

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