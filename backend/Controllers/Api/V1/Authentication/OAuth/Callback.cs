using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Attributes;
using ZapMe.Authentication.Models;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Helpers;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AuthController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="providerName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [AnonymousOnly]
    [EnableCors("allow_oauth_providers")]
    [RequestSizeLimit(1024)]
    [HttpPost("o/cb/{providerName}", Name = "OAuth Callback")]
    [Produces(Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    public async Task<IActionResult> OAuthCallback([FromRoute] string providerName, CancellationToken cancellationToken)
    {
        if (!await HttpContext.IsProviderSupportedAsync(providerName)) return CreateHttpError.UnsupportedOAuthProvider(providerName).ToActionResult();

        return Challenge(providerName);
    }
}
