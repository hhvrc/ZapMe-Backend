using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.BusinessLogic.Users.Relations;
using ZapMe.Database;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Block a user
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="204">User blocked</response>
    /// <response code="400">Invalid request</response>
    /// <response code="404">User not found</response>
    /// <returns></returns>
    [HttpPut("{userId}/block", Name = "BlockUser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Block([FromServices] DatabaseContext dbContext, [FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await BlockingLogic.ApplyUserBlock(dbContext, User.GetUserId(), userId, cancellationToken);

        return result.Match<IActionResult>(
                updated => updated ? NoContent() : NotFound(),
                error => error.ToActionResult()
        );
    }

    /// <summary>
    /// UnBlock a user
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="204">User blocked</response>
    /// <response code="400">Invalid request</response>
    /// <response code="404">User not found</response>
    /// <returns></returns>
    [HttpPut("{userId}/unblock", Name = "UnblockUser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnBlock([FromServices] DatabaseContext dbContext, [FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await BlockingLogic.RemoveUserBlock(dbContext, User.GetUserId(), userId, cancellationToken);

        return result.Match<IActionResult>(
            updated => updated ? NoContent() : NotFound(),
            error => error.ToActionResult()
        );
    }
}