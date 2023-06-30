using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.BusinessLogic.Users;
using ZapMe.Database;
using ZapMe.DTOs;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// List all incoming and outgoing friend requests
    /// </summary>
    [HttpGet("friendrequests", Name = "GetFriendRequests")]
    public Task<FriendRequestsDto> ListFriendRequests([FromServices] DatabaseContext dbContext, CancellationToken cancellationToken)
    {
        return UserRelationLogic.GetFriendRequestsDTO(dbContext, User.GetUserId(), cancellationToken);
    }
}