using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        int nDeleted = await _dbContext.Sessions.Where(s => s.Id == identity.SessionId).ExecuteDeleteAsync(cancellationToken);
        if (nDeleted < 0)
        {
            _logger.LogWarning("User {UserId} signed out but session {SessionId} was not found/deleted", identity.UserId, identity.SessionId);
        }

        return SignOut();
    }
}
