using Microsoft.AspNetCore.Mvc;
using ZapMe.DTOs;
using ZapMe.DTOs.SSO;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

public partial class SSOController
{
    /// <summary>
    /// Returns the data supplied by the SSO provider
    /// </summary>
    [HttpGet("providerData", Name = "SsoProviderdataGet")]
    [ProducesResponseType(typeof(ProviderDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Invalid SSO token
    public async Task<IActionResult> GetProviderData([FromQuery] string ssoToken, [FromServices] ISSOStateStore stateStore, CancellationToken cancellationToken)
    {
        SSOProviderDataEntry? providerVariables = await stateStore.GetProviderDataAsync(ssoToken, this.GetRemoteIP(), cancellationToken);
        if (providerVariables is null)
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
