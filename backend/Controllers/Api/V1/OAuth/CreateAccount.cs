using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.Attributes;
using ZapMe.Authentication.Models;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Controllers.Api.V1.OAuth.Models;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class OAuthController
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
    [HttpPost("create", Name = "OAuth Create Account")]
    [ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    public async Task<IActionResult> CreateAccount(
        [FromBody] OAuthCreateAccount body,
        [FromServices] ZapMeContext dbContext,
        [FromServices] IOAuthStateStore stateStore,
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

        var oauthVariables = await stateStore.GetRegistrationTicketAsync(body.OAuthTicket, requestingIp, cancellationToken);
        if (oauthVariables == null)
        {
            return CreateHttpError.Generic(
                StatusCodes.Status406NotAcceptable,
                "ticket_invalid",
                "The provided OAuth ticket is invalid or expired",
                "Please restart the OAuth flow by calling the /api/v1/auth/o/req endpoint"
            ).ToActionResult();
        }

        using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        ImageEntity? imageEntity = null;
        if (!String.IsNullOrEmpty(oauthVariables.ProfilePictureUrl))
        {
            var getOrCreateImageResult = await imageManager.GetOrCreateRecordAsync(
                oauthVariables.ProfilePictureUrl,
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
            Name = oauthVariables.Name,
            Email = oauthVariables.Email,
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
            return CreateHttpError.InternalServerError().ToActionResult();
        }

        if (imageEntity != null)
        {
            await dbContext.Images
                .Where(i => i.Id == imageEntity.Id)
                .ExecuteUpdateAsync(spc => spc.SetProperty(i => i.UploaderId, _ => user.Id), cancellationToken);
        }

        var connectionEntity = new OAuthConnectionEntity
        {
            UserId = user.Id,
            User = user,
            ProviderName = oauthVariables.Provider,
            ProviderId = oauthVariables.ProviderId,
        };

        await dbContext.OAuthConnections.AddAsync(connectionEntity, cancellationToken);
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