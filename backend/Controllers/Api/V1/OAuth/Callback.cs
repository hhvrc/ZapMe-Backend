using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Attributes;
using ZapMe.Authentication.Models;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

public partial class OAuthController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="providerName">Name of the OAuth provider to use, supported providers can be fetched from <see cref="OAuthController.ListOAuthProviders"/></param>
    /// <status code="200">Success, session has been created</status>
    /// <status code="406">Not Acceptable, the OAuth provider is not supported</status>
    [AnonymousOnly]
    [EnableCors("allow_oauth_providers")]
    [RequestSizeLimit(1024)]
    [HttpPost("{providerName}/callback", Name = "OAuth Callback")]
    [ProducesResponseType(typeof(OAuthResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ResponseCache(CacheProfileName = "no-store")]
    public async Task<IActionResult> Callback([FromRoute] string providerName)
    {
        if (!await HttpContext.IsProviderSupportedAsync(providerName)) return CreateHttpError.UnsupportedOAuthProvider(providerName).ToActionResult();

        return Challenge(providerName);
    }
}
