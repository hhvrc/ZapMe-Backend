using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ZapMe.Constants;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Options;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class ConfigController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="discordOAuth2Options"></param>
    /// <param name="githubOAuth2Options"></param>
    /// <param name="twitterOAuth2Options"></param>
    /// <param name="googleOptions"></param>
    /// <param name="turnstileOptions"></param>
    /// <returns>The config for the service</returns>
    /// <response code="200">Returns the service config</response>
    /// <returns></returns>
    [HttpGet(Name = "GetConfig")]
    [Produces(Application.Json)]
    [ProducesResponseType(typeof(Config.Models.Config), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    public Config.Models.Config GetConfig(
        [FromServices] IOptions<DiscordOAuth2Options> discordOAuth2Options,
        [FromServices] IOptions<GithubOAuth2Options> githubOAuth2Options,
        [FromServices] IOptions<TwitterOAuth2Options> twitterOAuth2Options,
        [FromServices] IOptions<GoogleOptions> googleOptions,
        [FromServices] IOptions<CloudflareTurnstileOptions> turnstileOptions
        )
    {
        return new Config.Models.Config
        {
            AppName = App.AppName,
            AppVersion = App.AppVersion.String,
            Api = new Config.Models.ApiConfig
            {
                TosVersion = 1,
                PrivacyVersion = 1,
                Authentication = new Config.Models.AuthenticationConfig
                {
                    DiscordClientId = discordOAuth2Options.Value.ClientID,
                    GithubClientId = githubOAuth2Options.Value.ClientID,
                    TwitterClientId = twitterOAuth2Options.Value.ClientID,
                    GoogleClientId = googleOptions.Value.OAuth2.ClientID,
                    RecaptchaSiteKey = googleOptions.Value.ReCaptcha.SiteKey,
                    TurnstileSiteKey = turnstileOptions.Value.SiteKey
                },
            },
            Contact = new Config.Models.ContactConfig
            {
                EmailSupport = App.SupportEmailAddress.ToString(),
                EmailContact = App.ContactEmailAddress.ToString(),
                DiscordInviteUrl = new Uri("https://discord.gg/ez6HE5vxe8")
            },
            FounderSocials = new Config.Models.SocialsConfig
            {
                GithubUri = new Uri("https://github.com/hhvrc"),
                TwitterUri = new Uri("https://twitter.com/hhvrc"),
                RedditUri = new Uri("https://reddit.com/u/hhvrc"),
                WebsiteUri = new Uri("https://heavenvr.tech")
            }
        };
    }
}
