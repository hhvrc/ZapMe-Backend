using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.Database.Models;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Add a sso connection to account
    /// </summary>
    [HttpPost("sso", Name = "Connect SSO Provider")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)] // SSO ticket not found
    public async Task<IActionResult> SSOConnect(
        [FromQuery] string ssoToken,
        [FromServices] ISSOStateStore stateStore,
        CancellationToken cancellationToken
        )
    {
        UserEntity? user = await User.GetUserAsync(_dbContext, cancellationToken);
        if (user == null)
        {
            return HttpErrors.UnauthorizedActionResult;
        }

        string requestingIp = this.GetRemoteIP();

        var oauthVariables = await stateStore.GetProviderDataAsync(ssoToken, requestingIp, cancellationToken);
        if (oauthVariables == null)
        {
            return HttpErrors.InvalidSSOTokenActionResult;
        }

        var connectionEntity = new SSOConnectionEntity
        {
            UserId = user.Id,
            User = user,
            ProviderName = oauthVariables.ProviderName,
            ProviderUserId = oauthVariables.ProviderUserId,
            ProviderUserName = oauthVariables.ProviderUserName,
        };

        await _dbContext.SSOConnections.AddAsync(connectionEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}