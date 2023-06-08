using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

public partial class SSOController
{
    /// <summary>
    /// Warning:
    /// This endpoint is not meant to be called by a 3rd party, it will redirect the request for OAuth authentication with the specified provider.
    /// It is documented here for transparency, and to automatically generate the OpenAPI client for the frontend.
    /// </summary>
    /// <param name="providerName">Name of the SSO provider to use, supported providers can be fetched from /api/v1/sso/providers</param>
    /// <status code="406">Not Acceptable, the SSO provider is not supported</status>
    [EnableCors("allow_sso_providers")]
    [HttpGet("{providerName}", Name = "SSO Authenticate")]
    [HttpPost("{providerName}", Name = "SSO Callback")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    public async Task<IActionResult> Authenticate([FromRoute] string providerName)
    {
        if (!await HttpContext.IsOAuthProviderSupportedAsync(providerName)) return HttpErrors.UnsupportedSSOProvider(providerName).ToActionResult();

        return Challenge(providerName);
    }
}
