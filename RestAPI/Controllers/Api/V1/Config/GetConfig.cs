using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.Constants;

namespace ZapMe.Controllers.Api.V1;

public partial class ConfigController
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns>The config for the service</returns>
    /// <response code="200">Returns the service config</response>
    /// <returns></returns>
    [HttpGet(Name = "GetConfig")]
    [ProducesResponseType(typeof(Config.Models.Config), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<Config.Models.Config> GetConfig(CancellationToken cancellationToken)
    {
        var privacyPolicy = await _dbContext
            .PrivacyPolicyDocuments
            .Where(pp => pp.IsActive)
            .OrderByDescending(pp => pp.Version)
            .Select(pp => new { pp.Version, pp.Markdown })
            .FirstAsync(cancellationToken);

        var termsOfService = await _dbContext
            .TermsOfServiceDocuments
            .Where(tos => tos.IsActive)
            .OrderByDescending(tos => tos.Version)
            .Select(tos => new { tos.Version, tos.Markdown })
            .FirstAsync(cancellationToken);

        return new Config.Models.Config
        {
            AppName = App.AppName,
            AppVersion = App.AppVersion.String,
            PrivacyPolicyVersion = privacyPolicy.Version,
            PrivacyPolicyMarkdown = privacyPolicy.Markdown,
            TermsOfServiceVersion = termsOfService.Version,
            TermsOfServiceMarkdown = termsOfService.Markdown,
            Api = new Config.Models.ApiConfig
            {
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
                WebsiteUri = new Uri("https://heavenvr.tech"),
                DiscordUsername = "HentaiHeaven#0001"
            }
        };
    }
}
