using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.BusinessRules;
using ZapMe.DTOs;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Look up user by name
    /// </summary>
    [RequestSizeLimit(1024)]
    [HttpGet("lookup", Name = "GetUserByName")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]     // Accepted
    [ProducesResponseType(StatusCodes.Status404NotFound)] // User not found
    public async Task<IActionResult> LookUp([FromServices] IUserRepository userRepository, [FromQuery] string username, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByUserNameAsync(username, cancellationToken);

        if (user is null)
        {
            return HttpErrors.UserNotFoundActionResult;
        }

        var userRelation = user.RelationsIncoming.FirstOrDefault(r => r.FromUserId == User.GetUserId());

        return UserRelationRules.IsEitherUserBlocking(user, User.GetUserId())
            ? Ok(UserMapper.MapToDto(user, userRelation))
            : Ok(UserMapper.MapToMinimalDto(user, userRelation));
    }
}