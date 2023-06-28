using Microsoft.AspNetCore.Mvc;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Remove a sso connection from account
    /// </summary>
    /// <returns></returns>
    [HttpDelete("sso", Name = "SsoProviderDisconnect")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Account does not have this SSO provider connected
    public IActionResult SSODisconnect([FromQuery] string providerName)
    {
        return Ok();
    }
}