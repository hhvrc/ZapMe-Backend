using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.BusinessLogic.Users;
using ZapMe.BusinessRules;
using ZapMe.DTOs;
using ZapMe.Enums;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Websocket;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Look up user by name
    /// </summary>
    /// <!-- <response code="302">User found</response> TODO: OpenAPI doesn't support 302 responses -->
    /// <response code="200">User found</response>
    /// <response code="404">User not found</response>
    [HttpGet("lookup", Name = "GetUserByName")]
    //[ProducesResponseType(typeof(UserDto), StatusCodes.Status302Found)] // TODO: OpenAPI doesn't support 302 responses
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LookUp([FromQuery] string username, [FromServices] IUserRepository userRepository, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserByUserNameAsync(username, cancellationToken);
        if (user is null)
        {
            return HttpErrors.UserNotFoundActionResult;
        }

        user.Status = WebSocketHub.IsUserOnline(user.Id) ? user.Status : UserStatus.Offline;

        //return RedirectToAction(nameof(Get), new { userId = user.Id }); // TODO: OpenAPI doesn't support 302 responses
        Guid selfUserId = User.GetUserId();
        var relation = UserRelationLogic.GetUserRelationDto(selfUserId, user);
        return UserRelationRules.IsEitherUserBlocking(user, selfUserId)
            ? Ok(UserMapper.MapToMinimalDto(user, relation))
            : Ok(UserMapper.MapToDto(user, relation));
    }
}