using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AuthenticationController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Returns users account</response>
    /// <response code="401">Error details</response>
    /// <response code="403">Error details</response>
    [RequestSizeLimit(1024)]
    [HttpPost("signin-oauth", Name = "AuthSignInOAuth")]
    [Produces(Application.Json, Application.Xml)]
    [ProducesResponseType(typeof(Account.Models.AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SignInOAuth([FromQuery] string provider, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return this.Error_AnonymousOnly();
        }

        if (!await HttpContext.IsProviderSupportedAsync(provider))
        {
            return this.Error(StatusCodes.Status406NotAcceptable, "Provider not supported", $"The OAuth provider \"{provider}\" is not supported", "Get the list of supported providers from the /api/v1/auth/providers endpoint");
        }

        return Challenge(provider);
    }
}
