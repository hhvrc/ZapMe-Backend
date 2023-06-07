using Microsoft.AspNetCore.Mvc;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

public partial class OAuthController
{
    /// <summary>
    /// Start the OAuth authentication process
    /// </summary>
    /// <param name="providerName">Name of the OAuth provider to use, supported providers can be fetched from <see cref="OAuthController.ListOAuthProviders"/></param>
    /// <status code="302">Success, redirects to OAuth provider's login page</status>
    /// <status code="406">Not Acceptable, the OAuth provider is not supported</status>
    [HttpGet("{providerName}/authenticate", Name = "OAuth Authenticate")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    public async Task<IActionResult> Authenticate([FromRoute] string providerName)
    {
        if (!await HttpContext.IsProviderSupportedAsync(providerName)) return CreateHttpError.UnsupportedOAuthProvider(providerName).ToActionResult();

        return Challenge(providerName);
    }
}
