using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Config.Models;

public struct AuthenticationConfig
{
    /// <summary>
    /// Discord Client ID for OAuth, if null, Discord login will be disabled
    /// </summary>
    [JsonPropertyName("discord_client_id")]
    public string? DiscordClientId { get; set; }

    /// <summary>
    /// Github Client ID for OAuth, if null, Github login will be disabled
    /// </summary>
    [JsonPropertyName("github_client_id")]
    public string? GithubClientId { get; set; }

    /// <summary>
    /// Twitter Client ID for OAuth, if null, Twitter login will be disabled
    /// </summary>
    [JsonPropertyName("twitter_client_id")]
    public string? TwitterClientId { get; set; }

    /// <summary>
    /// Google Client ID for OAuth, if null, Google login will be disabled
    /// </summary>
    [JsonPropertyName("google_client_id")]
    public string? GoogleClientId { get; set; }

    /// <summary>
    /// ReCaptcha site key for bot detection, if null, ReCaptcha will be disabled
    /// </summary>
    [JsonPropertyName("recaptcha_site_key")]
    public string? RecaptchaSiteKey { get; set; }

    /// <summary>
    /// Turnstile site key for bot detection, if null, Turnstile will be disabled
    /// </summary>
    [JsonPropertyName("turnstile_site_key")]
    public string? TurnstileSiteKey { get; set; }
}
