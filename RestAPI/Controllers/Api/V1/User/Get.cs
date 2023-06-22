using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Enums;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Get user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [RequestSizeLimit(1024)]
    [HttpGet("i/{userId}", Name = "GetUser")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)] // Accepted
    [ProducesResponseType(StatusCodes.Status404NotFound)]            // User not found
    public async Task<IActionResult> Get([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        Guid thisUserId = User.GetUserId();

        var user = await _userManager.GetByIdAsync(thisUserId, userId, cancellationToken);
        if (user is null)
        {
            return HttpErrors.UserNotFoundActionResult;
        }

        UserRelationEntity? outgoingRelation = user.RelationsIncoming.Where(r => r.SourceUserId == thisUserId).FirstOrDefault();

        return Ok(user.ToUserDto(outgoingRelation));
    }
}