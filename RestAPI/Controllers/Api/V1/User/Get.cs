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
    [RequestSizeLimit(1024)]
    [HttpGet("{userId}", Name = "GetUserById")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)] // Accepted
    [ProducesResponseType(StatusCodes.Status404NotFound)]            // User not found
    public async Task<IActionResult> Get([FromServices] IUserRepository userRepository, [FromRoute] Guid userId, CancellationToken cancellationToken)
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