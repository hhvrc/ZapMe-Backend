using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

public partial class SSOController
{
    /// <summary>
    /// Warning: This endpoint is not meant to be called by API clients, but only by the frontend.
    /// SSO authentication endpoint
    /// </summary>
    /// <param name="providerName">Name of the SSO provider to use, supported providers can be fetched from /api/v1/sso/providers</param>
    /// <status code="406">Not Acceptable, the SSO provider is not supported</status>
    [EnableCors("allow_sso_providers")]
    [HttpGet("{providerName}", Name = "InternalSsoAuthenticate")]
    [HttpPost("{providerName}/callback", Name = "InternalSsoCallback")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    public async Task<IActionResult> Authenticate([FromRoute] string providerName)
    {
        if (!await HttpContext.IsOAuthProviderSupportedAsync(providerName)) return HttpErrors.UnsupportedSSOProvider(providerName).ToActionResult();

        return Challenge(providerName);
    }
}
