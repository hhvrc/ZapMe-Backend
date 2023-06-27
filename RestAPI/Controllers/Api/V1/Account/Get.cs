using Amazon.Auth.AccessControlPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Account</response>
    [HttpGet(Name = "GetAccount")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        Guid userId = User.GetUserId();

        UserEntity? user = await _dbContext
            .Users
            .Include(u => u.ProfileAvatar)
            .Include(u => u.ProfileBanner)
            .Include(u => u.RelationsOutgoing)
            .Include(u => u.SSOConnections)
            .SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
        {
            return HttpErrors.UnauthorizedActionResult;
        }

        return Ok(user.ToAccountDto());
    }
}