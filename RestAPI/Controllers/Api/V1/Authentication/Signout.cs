using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZapMe.Authentication;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

public partial class AuthController
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
        Guid? sessionId = User.GetSessionId();
        if (!sessionId.HasValue) return HttpErrors.UnauthorizedActionResult;

        int nDeleted = await _dbContext.Sessions
            .Where(s => s.Id == sessionId)
            .ExecuteDeleteAsync(cancellationToken);
        if (nDeleted < 0)
        {
            _logger.LogWarning("User {UserId} signed out but session {SessionId} was not found/deleted", User.GetUserId(), sessionId);
        }

        return SignOut();
    }
}
