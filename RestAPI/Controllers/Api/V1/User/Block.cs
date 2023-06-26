using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.Enums;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Block a user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="204">User blocked</response>
    /// <response code="400">Invalid request</response>
    /// <response code="404">User not found</response>
    /// <returns></returns>
    [HttpPut("block/{userId}", Name = "Block User")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Block([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        Guid authenticatedUserId = User.GetUserId();

        if (authenticatedUserId == userId)
            return BadRequest();

        bool success = await _userManager.SetUserRelationTypeAsync(authenticatedUserId, userId, UserRelationType.Blocked, cancellationToken);

        // TODO: raise notification

        return success ? NoContent() : NotFound();
    }
}