using System.Text.Json.Serialization;

namespace ZapMe.Controllers.Api.V1.Config.Models;

public struct ApiDisabledFeatures
{
    [JsonPropertyName("chat")]
    public bool Chat { get; set; }

    [JsonPropertyName("sessions")]
    public bool Sessions { get; set; }

    [JsonPropertyName("webrtc")]
    public bool WebRTC { get; set; }

    [JsonPropertyName("websockets")]
    public bool WebSockets { get; set; }

    [JsonPropertyName("captcha")]
    public bool Captcha { get; set; }

    [JsonPropertyName("registration")]
    public bool Registration { get; set; }

    [JsonPropertyName("password_reset")]
    public bool PasswordReset { get; set; }

    [JsonPropertyName("email_verification")]
    public bool EmailVerification { get; set; }
}