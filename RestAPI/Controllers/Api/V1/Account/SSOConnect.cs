using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        Guid userId = User.GetUserId();

        string requestingIp = this.GetRemoteIP();

        var oauthVariables = await stateStore.GetProviderDataAsync(ssoToken, requestingIp, cancellationToken);
        if (oauthVariables is null)
        {
            return HttpErrors.InvalidSSOTokenActionResult;
        }

        _dbContext.SSOConnections.Add(new SSOConnectionEntity
        {
            UserId = userId,
            ProviderName = oauthVariables.ProviderName,
            ProviderUserId = oauthVariables.ProviderUserId,
            ProviderUserName = oauthVariables.ProviderUserName,
        });
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}