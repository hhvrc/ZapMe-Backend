using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Authentication;

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
        ZapMeIdentity identity = (User.Identity as ZapMeIdentity)!;

        await _sessionManager.SignOutAsync(identity.SessionId, cancellationToken);

        return SignOut();
    }
}
