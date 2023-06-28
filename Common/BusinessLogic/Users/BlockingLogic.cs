﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.Enums;

namespace ZapMe.BusinessLogic.Users;

public static class BlockingLogic
{
    public enum ActionResult
    {
        Ok,
        Unchanged,
        CannotModerateSelf
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="sourceUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Boolean inicating if a change has occured, or a ErrorDetails struct containing error</returns>
    public static async Task<ActionResult> ApplyUserBlock(DatabaseContext dbContext, Guid sourceUserId, Guid targetUserId, CancellationToken cancellationToken)
    {
        if (sourceUserId == targetUserId) return ActionResult.CannotModerateSelf;

        using IDbContextTransaction? transaction = await dbContext.Database.BeginTransactionIfNotExistsAsync(cancellationToken);

        // Crate or update the outgoing relation to be blocked
        UserRelationEntity? outgouing = await dbContext
            .UserRelations
            .AsTracking()
            .FirstOrDefaultAsync(u => u.SourceUserId == sourceUserId && u.TargetUserId == targetUserId, cancellationToken);
        if (outgouing is null)
        {
            dbContext.UserRelations.Add(new UserRelationEntity
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId,
                RelationType = UserRelationType.Blocked
            });
        }
        else
        {
            outgouing.RelationType = UserRelationType.Blocked;
        }
        int nChanges = await dbContext.SaveChangesAsync(cancellationToken);

        // Update the incoming relation to be none if it is friend
        await dbContext
            .UserRelations
            .Where(u => u.SourceUserId == targetUserId && u.TargetUserId == sourceUserId && u.RelationType == UserRelationType.Friend)
            .ExecuteUpdateAsync(spc => spc.SetProperty(ur => ur.RelationType, UserRelationType.None), cancellationToken);

        if (transaction is not null)
        {
            await transaction.CommitAsync(cancellationToken);
        }

        // TODO: raise notification

        return nChanges > 0
            ? ActionResult.Ok
            : ActionResult.Unchanged;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="sourceUserId"></param>
    /// <param name="targetUserId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Boolean inicating if a change has occured, or a ErrorDetails struct containing error</returns>
    public static async Task<ActionResult> RemoveUserBlock(DatabaseContext dbContext, Guid sourceUserId, Guid targetUserId, CancellationToken cancellationToken)
    {
        if (sourceUserId == targetUserId) return ActionResult.CannotModerateSelf;

        // Apply outgoing relation to be none only if it's blocked
        int nChanges = await dbContext
            .UserRelations
            .Where(u => u.SourceUserId == sourceUserId && u.TargetUserId == targetUserId && u.RelationType == UserRelationType.Blocked)
            .ExecuteUpdateAsync(spc => spc.SetProperty(ur => ur.RelationType, UserRelationType.None), cancellationToken);

        // TODO: raise notification

        return nChanges > 0
            ? ActionResult.Ok
            : ActionResult.Unchanged;
    }
}