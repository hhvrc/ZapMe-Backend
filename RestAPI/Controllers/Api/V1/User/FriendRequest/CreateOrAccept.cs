using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.DTOs.API.User;
using ZapMe.Enums;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Create a new friend request or accept an incoming friend request to this user
    /// </summary>
    /// <response code="200">Created/Accepted request</response>
    /// <response code="304">Friend request already exists</response>
    /// <response code="400">Bad request/Not allowed/Already friends</response>
    [HttpPut("{userId}/friendrequest", Name = "CreateOrAcceptFriendRequest")]
    [ProducesResponseType(typeof(FriendRequestCreateOrAccept200OkDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(typeof(FriendRequestCreateOrAccept400BadRequestDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> FriendRequestCreateOrAccept([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await _userManager.CreateOrAcceptFriendRequestAsync(User.GetUserId(), userId, cancellationToken);

        return result switch
        {
            CreateOrAcceptFriendRequestResult.Success => Ok(new FriendRequestCreateOrAccept200OkDto()),
            CreateOrAcceptFriendRequestResult.NoChanges => StatusCode(StatusCodes.Status304NotModified),
            CreateOrAcceptFriendRequestResult.NotAllowed => BadRequest(), // TODO: Create a response DTO for this
            CreateOrAcceptFriendRequestResult.AlreadyFriends => BadRequest(), // TODO: Create a response DTO for this
            CreateOrAcceptFriendRequestResult.FriendshipCreated => Ok(new FriendRequestCreateOrAccept200OkDto()),
            CreateOrAcceptFriendRequestResult.CannotApplyToSelf => BadRequest(), // TODO: Create a response DTO for this
            _ => HttpErrors.InternalServerErrorActionResult,
        };
    }
}