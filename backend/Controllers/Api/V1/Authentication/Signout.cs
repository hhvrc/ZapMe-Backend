using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Data.Models;

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
        SessionEntity session = this.GetSignIn()!;

        await _sessionManager.SignOutAsync(session.Id, cancellationToken);

        return SignOut();
    }
}
