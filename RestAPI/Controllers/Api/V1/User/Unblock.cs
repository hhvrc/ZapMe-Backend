using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.Enums;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Unblock a user
    /// </summary>
    /// <response code="204">User unblocked</response>
    /// <response code="304">User was already unblocked</response>
    /// <response code="400">You can't moderate yourself</response>
    [HttpPut("{userId}/unblock", Name = "UnblockUser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UnBlock([FromRoute] Guid userId, [FromServices] IUserRelationManager userRelationManager, CancellationToken cancellationToken)
    {
        var result = await userRelationManager.UnblockUserAsync(User.GetUserId(), userId, cancellationToken);

        return result switch
        {
            UpdateUserRelationResult.Success => NoContent(),
            UpdateUserRelationResult.NoChanges => StatusCode(StatusCodes.Status304NotModified),
            UpdateUserRelationResult.CannotApplyToSelf => BadRequest(),
            _ => HttpErrors.InternalServerErrorActionResult,
        };
    }
}