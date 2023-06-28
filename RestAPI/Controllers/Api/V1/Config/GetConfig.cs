using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.Constants;
using ZapMe.DTOs.Config;

namespace ZapMe.Controllers.Api.V1;

public partial class ConfigController
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns>The config for the service</returns>
    /// <response code="200">Returns the service config</response>
    /// <returns></returns>
    [HttpGet(Name = "GetApiConfig")]
    [ProducesResponseType(typeof(ApiConfig), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ApiConfig> GetConfig(CancellationToken cancellationToken)
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

        return new ApiConfig
        {
            AppName = App.AppName,
            AppVersion = App.AppVersion.String,
            PrivacyPolicyVersion = privacyPolicy.Version,
            PrivacyPolicyMarkdown = privacyPolicy.Markdown,
            TermsOfServiceVersion = termsOfService.Version,
            TermsOfServiceMarkdown = termsOfService.Markdown,
            WebRtc = new WebRtcConfig
            {
                Enabled = true, // TODO: Make this configurable
                ApprovedStunServers = new string[] // TODO: Make this configurable
                {
                    "stun.l.google.com:19302",
                    "stun1.l.google.com:19302",
                    "stun2.l.google.com:19302",
                    "stun3.l.google.com:19302",
                    "stun4.l.google.com:19302",
                }
            },
            Contact = new ContactConfig
            {
                EmailSupport = App.SupportEmailAddress.ToString(),
                EmailContact = App.ContactEmailAddress.ToString(),
                DiscordInviteUrl = new Uri("https://discord.gg/ez6HE5vxe8")
            },
            FounderSocials = new SocialsConfig
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
