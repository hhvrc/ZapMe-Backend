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
    /// <returns>The config for the service</returns>
    /// <response code="200">Returns the service config</response>
    [HttpGet(Name = "GetConfig")]
    [Produces(Application.Json, Application.Xml)]
    [ProducesResponseType(typeof(Config.Models.Config), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public Config.Models.Config GetConfig()
    {
        return new Config.Models.Config {
            AppName = App.AppName,
            AppVersion = App.AppVersion.String,
            Api = new Config.Models.ApiConfig
            {
                TosVersion = 1,
                PrivacyVersion = 1,
            },
            Contact = new Config.Models.ContactConfig
            {
                EmailSupport = App.SupportMailAddress.ToString(),
                EmailContact = App.ContactMailAddress.ToString(),
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
