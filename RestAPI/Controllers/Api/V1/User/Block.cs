﻿using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.Enums;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Block a user
    /// </summary>
    /// <response code="204">User blocked</response>
    /// <response code="304">User already blocked</response>
    /// <response code="400">You can't moderate yourself</response>
    [HttpPut("{userId}/block", Name = "BlockUser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Block([FromRoute] Guid userId, [FromServices] IUserRelationManager userRelationManager, CancellationToken cancellationToken)
    {
        var result = await userRelationManager.BlockUserAsync(User.GetUserId(), userId, cancellationToken);

        return result switch
        {
            UpdateUserRelationResult.Success => NoContent(),
            UpdateUserRelationResult.NoChanges => StatusCode(StatusCodes.Status304NotModified),
            UpdateUserRelationResult.CannotApplyToSelf => BadRequest(),
            _ => HttpErrors.InternalServerErrorActionResult,
        };
    }
}