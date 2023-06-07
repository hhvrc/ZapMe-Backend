using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Authentication;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class OAuthController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="oauthTicket"></param>
    /// <param name="stateStore"></param>
    /// <param name="dbContext"></param>
    /// <param name="cancellationToken"></param>
    /// <status code="200"></status>
    [Authorize(ZapMeAuthenticationDefaults.AuthenticationScheme)]
    [Consumes(Application.Json)]
    [HttpPost("connect", Name = "OAuth Connect Account")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    public async Task<IActionResult> ConnectAccount(
        [FromQuery] string oauthTicket,
        [FromServices] IOAuthStateStore stateStore,
        [FromServices] ZapMeContext dbContext,
        CancellationToken cancellationToken
        )
    {
        ZapMeIdentity identity = (User as ZapMePrincipal)!.Identity;
        UserEntity user = identity.User;

        string requestingIp = this.GetRemoteIP();

        var oauthVariables = await stateStore.GetRegistrationTicketAsync(oauthTicket, requestingIp, cancellationToken);
        if (oauthVariables == null)
        {
            return CreateHttpError.Generic(
                StatusCodes.Status406NotAcceptable,
                "ticket_invalid",
                "The provided OAuth ticket is invalid or expired",
                "Please restart the OAuth flow by calling the /api/v1/auth/o/req endpoint"
            ).ToActionResult();
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

        return Ok();
    }
}