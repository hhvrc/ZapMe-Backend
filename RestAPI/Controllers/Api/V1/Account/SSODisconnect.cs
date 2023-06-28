using Microsoft.AspNetCore.Mvc;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Warning: This endpoint is not meant to be called by API clients, but only by the frontend.
    /// Remove a sso connection from account
    /// </summary>
    /// <returns></returns>
    [HttpDelete("sso", Name = "InternalDisconnectSsoProvider")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)] // Account does not have this SSO provider connected
    public IActionResult SSODisconnect([FromQuery] string providerName)
    {
        return Ok();
    }
}