using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.SSO.Models;
using ZapMe.DTOs;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

public partial class SSOController
{
    /// <summary>
    /// Returns the data supplied by the SSO provider
    /// </summary>
    [HttpGet("providerData", Name = "SSO Get Provider Data")]
    [ProducesResponseType(typeof(ProviderDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Invalid SSO token
    public async Task<IActionResult> GetProviderData([FromQuery] string ssoToken, [FromServices] ISSOStateStore stateStore, CancellationToken cancellationToken)
    {
        SSOProviderDataEntry? providerVariables = await stateStore.GetRegistrationTokenAsync(ssoToken, this.GetRemoteIP(), cancellationToken);
        if (providerVariables == null)
        {
            return HttpErrors.InvalidSSOTokenActionResult;
        }

        return Ok(new ProviderDataDto
        {
            ProviderName = providerVariables.ProviderName,
            UserName = providerVariables.ProviderUserName,
            Email = providerVariables.ProviderUserEmail,
            EmailVerified = providerVariables.ProviderUserEmailVerified,
            ExpiresAtUtc = providerVariables.ExpiresAt
        });
    }
};
