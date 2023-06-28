using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.BusinessLogic.Users;
using ZapMe.Database;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Block a user
    /// </summary>
    /// <response code="204">User blocked</response>
    /// <response code="400">You can't moderate yourself</response>
    [HttpPut("{userId}/block", Name = "BlockUser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Block([FromServices] DatabaseContext dbContext, [FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await BlockingLogic.ApplyUserBlock(dbContext, User.GetUserId(), userId, cancellationToken);

        return result switch
        {
            BlockingLogic.ActionResult.Ok => NoContent(),
            BlockingLogic.ActionResult.Unchanged => NoContent(),
            BlockingLogic.ActionResult.CannotModerateSelf => BadRequest(),
            _ => HttpErrors.InternalServerErrorActionResult,
        };
    }

    /// <summary>
    /// Unblock a user
    /// </summary>
    /// <response code="204">User unblocked</response>
    /// <response code="400">You can't moderate yourself</response>
    [HttpPut("{userId}/unblock", Name = "UnblockUser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UnBlock([FromServices] DatabaseContext dbContext, [FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await BlockingLogic.RemoveUserBlock(dbContext, User.GetUserId(), userId, cancellationToken);

        return result switch
        {
            BlockingLogic.ActionResult.Ok => NoContent(),
            BlockingLogic.ActionResult.Unchanged => NoContent(),
            BlockingLogic.ActionResult.CannotModerateSelf => BadRequest(),
            _ => HttpErrors.InternalServerErrorActionResult,
        };
    }
}