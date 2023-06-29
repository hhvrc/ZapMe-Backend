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
    /// Get user by Id
    /// </summary>
    /// <response code="404">User not found</response>
    [HttpGet("{userId}", Name = "GetUserById")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid userId, [FromServices] IUserRepository userRepository, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return HttpErrors.UserNotFoundActionResult;
        }

        var userRelation = user.RelationsIncoming.FirstOrDefault(r => r.FromUserId == User.GetUserId());

        return UserRelationRules.IsEitherUserBlocking(user, User.GetUserId())
            ? Ok(UserMapper.MapToMinimalDto(user, userRelation))
            : Ok(UserMapper.MapToDto(user, userRelation));
    }
}