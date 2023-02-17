using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Config.Models;

public struct AuthenticationConfig
{
    /// <summary>
    /// Google Client ID for OAuth, if null, Google login will be disabled
    /// </summary>
    [JsonPropertyName("google_client_id")]
    public string? GoogleClientId { get; set; }

    /// <summary>
    /// Github Client ID for OAuth, if null, Github login will be disabled
    /// </summary>
    [JsonPropertyName("github_client_id")]
    public string? GithubClientId { get; set; }

    /// <summary>
    /// ReCaptcha Client ID for bot detection, if null, ReCaptcha will be disabled
    /// </summary>
    [JsonPropertyName("recaptcha_site_key")]
    public string? RecaptchaSiteKey { get; set; }
}
