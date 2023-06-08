using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.Attributes;
using ZapMe.Authentication.Models;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Controllers.Api.V1.SSO.Models;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class SSOController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="body"></param>
    /// <param name="dbContext"></param>
    /// <param name="stateStore"></param>
    /// <param name="imageManager"></param>
    /// <param name="userStore"></param>
    /// <param name="sessionManager"></param>
    /// <param name="cancellationToken"></param>
    /// <status code="200"></status>
    [AnonymousOnly]
    [RequestSizeLimit(1024)]
    [Consumes(Application.Json)]
    [HttpPost("create", Name = "SSO Create Account")]
    [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    public async Task<IActionResult> CreateAccount(
        [FromBody] SSOCreateAccount body,
        [FromServices] ZapMeContext dbContext,
        [FromServices] ISSOStateStore stateStore,
        [FromServices] IImageManager imageManager,
        [FromServices] IUserStore userStore,
        [FromServices] ISessionManager sessionManager,
        CancellationToken cancellationToken
        )
    {
        string requestingIp = this.GetRemoteIP();
        string requestingCfIpCountry = this.GetCloudflareIPCountry();
        string requestingCfIpRegion = CountryRegionLookup.GetCloudflareRegion(requestingCfIpCountry);
        string reqestingUserAgent = this.GetRemoteUserAgent();

        var ssoVariables = await stateStore.GetRegistrationTokenAsync(body.SSOToken, requestingIp, cancellationToken);
        if (ssoVariables == null)
        {
            return HttpErrors.InvalidSSOTokenActionResult;
        }

        using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        ImageEntity? imageEntity = null;
        if (!String.IsNullOrEmpty(ssoVariables.ProfilePictureUrl))
        {
            var getOrCreateImageResult = await imageManager.GetOrCreateRecordAsync(
                ssoVariables.ProfilePictureUrl,
                requestingCfIpRegion,
                null,
                null,
                cancellationToken
                );
            if (getOrCreateImageResult.TryPickT1(out ErrorDetails errorDetails, out imageEntity))
            {
                return errorDetails.ToActionResult();
            }
        }

        var user = new UserEntity
        {
            Name = ssoVariables.ProviderName,
            Email = ssoVariables.ProviderUserEmail,
            EmailVerified = true,
            PasswordHash = PasswordUtils.HashPassword(body.Password),
            AcceptedPrivacyPolicyVersion = body.AcceptedPrivacyPolicyVersion,
            AcceptedTermsOfServiceVersion = body.AcceptedTermsOfServiceVersion,
            ProfilePictureId = imageEntity?.Id,
            OnlineStatus = Enums.UserStatus.Online,
            OnlineStatusText = "I'm online!",
        };
        if (!await userStore.TryCreateAsync(user, cancellationToken))
        {
            return HttpErrors.InternalServerErrorActionResult;
        }

        if (imageEntity != null)
        {
            await dbContext.Images
                .Where(i => i.Id == imageEntity.Id)
                .ExecuteUpdateAsync(spc => spc.SetProperty(i => i.UploaderId, _ => user.Id), cancellationToken);
        }

        var connectionEntity = new SSOConnectionEntity
        {
            UserId = user.Id,
            User = user,
            ProviderName = ssoVariables.ProviderName,
            ProviderUserId = ssoVariables.ProviderUserId,
            ProviderUserName = ssoVariables.ProviderUserName,
        };

        await dbContext.SSOConnections.AddAsync(connectionEntity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var session = await sessionManager.CreateAsync(
            user,
            requestingIp,
            requestingCfIpCountry,
            reqestingUserAgent,
            true, // TODO: Should RememberMe be true by default?
            cancellationToken
            );

        transaction.Commit();

        return Ok(new SessionDto(session));
    }
}