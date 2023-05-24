using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Attributes;
using ZapMe.Authentication.Models;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Helpers;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AuthenticationController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="providerName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Returns SignInOk along with a Cookie with similar data</response>
    /// <response code="401">Error details</response>
    /// <response code="403">Error details</response>
    [AnonymousOnly]
    [EnableCors("allow_oauth_providers")]
    [RequestSizeLimit(1024)]
    [HttpGet("o/req/{providerName}", Name = "OAuth Authorize")]
    [Produces(Application.Json)]
    [ProducesResponseType(typeof(SignInOk), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> OAuthAuthorize([FromRoute] string providerName, CancellationToken cancellationToken)
    {
        if (!await HttpContext.IsProviderSupportedAsync(providerName))
        {
            return CreateHttpError.Generic(StatusCodes.Status406NotAcceptable, "Provider not supported", $"The OAuth provider \"{providerName}\" is not supported", "Get the list of supported providers from the /api/v1/auth/o/list endpoint").ToActionResult();
        }

        return Challenge(providerName);
    }
}
