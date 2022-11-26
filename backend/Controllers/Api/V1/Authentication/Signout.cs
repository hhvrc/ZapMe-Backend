using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ZapMe.Controllers.Api.V1;

public partial class AuthenticationController
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Ok</response>
    [Authorize]
    [HttpPost("signout", Name = "AuthSignOut")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SignOut(CancellationToken cancellationToken)
    {
        Guid sessionId = ???; // TODO: Get the session ID of this login session

        await _sessionManager.SignOutAsync(session.Id, cancellationToken);

        return SignOut();
    }
}
