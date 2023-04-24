using Microsoft.AspNetCore.Mvc;
using ZapMe.Constants;
using ZapMe.Controllers.Api.V1.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class ConfigController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns>The config for the service</returns>
    /// <response code="200">Returns the service config</response>
    /// <returns></returns>
    [HttpGet(Name = "GetConfig")]
    [Produces(Application.Json)]
    [ProducesResponseType(typeof(Config.Models.Config), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    public Config.Models.Config GetConfig([FromServices] IConfiguration configuration)
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
                    DiscordClientId = configuration["Authorization:Discord:ClientId"],
                    GithubClientId = configuration["Authorization:Github:ClientId"],
                    TwitterClientId = configuration["Authorization:Twitter:ClientId"],
                    GoogleClientId = configuration["Authorization:Google:ClientId"],
                    RecaptchaSiteKey = configuration["Authorization:ReCaptcha:SiteKey"],
                    TurnstileSiteKey = configuration["Authorization:Turnstile:SiteKey"]
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
