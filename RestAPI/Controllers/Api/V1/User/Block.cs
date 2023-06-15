using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.DTOs.Moderation;
using ZapMe.Enums;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Block a user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("block", Name = "Block User")]
    [ProducesResponseType(StatusCodes.Status200OK)] // User blocked
    [ProducesResponseType(StatusCodes.Status404NotFound)] // User not found
    public async Task<IActionResult> Block([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        Guid authorizedUserId = User.GetUserId();

        if (authorizedUserId == userId)
            return HttpErrors.Generic(
                StatusCodes.Status400BadRequest,
                "invalid_action",
                "You cannot block yourself.", 
                NotificationSeverityLevel.Warning,
                "You cannot block yourself."
                ).ToActionResult();

        UserRelationEntity[] relations =
            await _dbContext
            .UserRelations
            .AsTracking()
            .Where(r => (r.SourceUserId == authorizedUserId && r.TargetUserId == userId) || (r.SourceUserId == userId && r.TargetUserId == authorizedUserId))
            .ToArrayAsync(cancellationToken);

        bool gotBlocked = false;
        foreach (UserRelationEntity relation in relations)
        {
            if (relation.SourceUserId == authorizedUserId)
            {
                relation.RelationType = UserRelationType.Blocked;
                gotBlocked = true;
            }
            else
            {
                relation.RelationType = UserRelationType.None;
            }
        }

        if (gotBlocked)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Ok();
        }

        UserRelationEntity userRelation = new()
        {
            SourceUserId = authorizedUserId,
            TargetUserId = userId,
            RelationType = UserRelationType.Blocked
        };
        await _dbContext.UserRelations.AddAsync(userRelation, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}