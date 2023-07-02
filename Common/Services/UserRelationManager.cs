using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ZapMe.Database;
using ZapMe.Database.Extensions;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.DTOs.Mediator;
using ZapMe.Enums;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class UserRelationManager : IUserRelationManager
{
    private readonly DatabaseContext _dbContext;
    private readonly IMediator _mediator;

    public UserRelationManager(DatabaseContext dbContext, IMediator mediator)
    {
        _dbContext = dbContext;
        _mediator = mediator;
    }

    private async Task SetFriendStatus(Guid fromUserId, Guid toUserId, UserPartialFriendStatus friendStatus, UserRelationEntity? trackedEntity, CancellationToken cancellationToken)
    {
        if (trackedEntity is null)
        {
            trackedEntity = new UserRelationEntity
            {
                FromUserId = fromUserId,
                ToUserId = toUserId,
                FriendStatus = friendStatus,
            };

            await _dbContext.UserRelations.AddAsync(trackedEntity, cancellationToken);
        }
        else
        {
            trackedEntity.FriendStatus = friendStatus;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SetFriendStatus(Guid fromUserId, Guid toUserId, UserPartialFriendStatus friendStatus, CancellationToken cancellationToken)
    {
        var trackedEntity = await _dbContext.UserRelations.AsTracking()
            .FirstOrDefaultAsync(x => x.FromUserId == fromUserId && x.ToUserId == toUserId, cancellationToken);

        await SetFriendStatus(fromUserId, toUserId, friendStatus, trackedEntity, cancellationToken);
    }

    public async Task<CreateOrAcceptFriendRequestResult> CreateOrAcceptFriendRequestAsync(Guid fromUserId, Guid toUserId, CancellationToken cancellationToken)
    {
        if (fromUserId == toUserId) return CreateOrAcceptFriendRequestResult.CannotApplyToSelf;

        // Get all relations between the two users (if any)
        var relations = await _dbContext.UserRelations.AsTracking()
            .Where(x => (x.FromUserId == fromUserId && x.ToUserId == toUserId) || (x.FromUserId == toUserId && x.ToUserId == fromUserId))
            .Take(2).ToArrayAsync(cancellationToken);

        // Check if the users are already friends or blocked, if so, don't create a friend request
        if (relations.Any(x => x.FriendStatus == UserPartialFriendStatus.Blocked)) return CreateOrAcceptFriendRequestResult.NotAllowed;
        if (relations.Any(x => x.FriendStatus == UserPartialFriendStatus.Accepted)) return CreateOrAcceptFriendRequestResult.AlreadyFriends;

        // Seperate the relations into incoming and outgoing
        var outgoingRelation = relations.FirstOrDefault(x => x.FromUserId == fromUserId);
        var incomingRelation = relations.FirstOrDefault(x => x.FromUserId == toUserId);

        // If there's a pending incoming friend request, make the two users friends
        if (incomingRelation is not null && incomingRelation.FriendStatus == UserPartialFriendStatus.Pending)
        {
            using IDbContextTransaction? transaction = await _dbContext.Database.BeginTransactionIfNotExistsAsync(cancellationToken);

            // Force the friend status to accepted
            incomingRelation.FriendStatus = UserPartialFriendStatus.Accepted;
            await SetFriendStatus(fromUserId, toUserId, UserPartialFriendStatus.Accepted, outgoingRelation, cancellationToken);

            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            // Publish the friendship created event
            await _mediator.Publish(new UserFriendshipCreatedEvent(fromUserId, toUserId), cancellationToken);

            return CreateOrAcceptFriendRequestResult.FriendshipCreated;
        }

        // If there's a pending outgoing friend request, do nothing
        if (outgoingRelation is not null && outgoingRelation.FriendStatus == UserPartialFriendStatus.Pending) return CreateOrAcceptFriendRequestResult.NoChanges;

        // Set the friend status to pending
        await SetFriendStatus(fromUserId, toUserId, UserPartialFriendStatus.Pending, outgoingRelation, cancellationToken);

        // Publish the friend request created event
        await _mediator.Publish(new UserFriendRequestCreatedEvent(fromUserId, toUserId), cancellationToken);

        return CreateOrAcceptFriendRequestResult.Success;
    }

    public async Task<UpdateUserRelationResult> DeleteOrRejectFriendRequestAsync(Guid fromUserId, Guid toUserId, CancellationToken cancellationToken)
    {
        if (fromUserId == toUserId) return UpdateUserRelationResult.CannotApplyToSelf;

        int nRemoved = await _dbContext
            .UserRelations
            .Where(fr =>
                (
                    (fr.FromUserId == fromUserId && fr.ToUserId == toUserId) ||
                    (fr.FromUserId == toUserId && fr.ToUserId == fromUserId)
                )
                && fr.FriendStatus == UserPartialFriendStatus.Pending
            )
            .ExecuteUpdateAsync(spc => spc.SetProperty(x => x.FriendStatus, UserPartialFriendStatus.None), cancellationToken);

        if (nRemoved <= 0)
        {
            return UpdateUserRelationResult.NoChanges;
        }

        await _mediator.Publish(new UserFriendRequestDeletedEvent(fromUserId, toUserId), cancellationToken);

        return UpdateUserRelationResult.Success;
    }

    public async Task<UpdateUserRelationResult> RemoveFriendshipAsync(Guid fromUserId, Guid toUserId, CancellationToken cancellationToken)
    {
        if (fromUserId == toUserId) return UpdateUserRelationResult.CannotApplyToSelf;

        int nRemoved = await _dbContext
            .UserRelations
            .Where(fr =>
                (
                    (fr.FromUserId == fromUserId && fr.ToUserId == toUserId) ||
                    (fr.FromUserId == toUserId && fr.ToUserId == fromUserId)
                )
                && (fr.FriendStatus == UserPartialFriendStatus.Pending || fr.FriendStatus == UserPartialFriendStatus.Accepted)
            )
            .ExecuteUpdateAsync(spc => spc.SetProperty(x => x.FriendStatus, UserPartialFriendStatus.None), cancellationToken);

        if (nRemoved <= 0)
        {
              return UpdateUserRelationResult.NoChanges;
        }

        await _mediator.Publish(new UserFriendshipDeletedEvent(fromUserId, toUserId), cancellationToken);

        return UpdateUserRelationResult.Success;
    }

    public async Task<UpdateUserRelationResult> BlockUserAsync(Guid fromUserId, Guid toUserId, CancellationToken cancellationToken)
    {
        if (fromUserId == toUserId) return UpdateUserRelationResult.CannotApplyToSelf;

        using IDbContextTransaction? transaction = await _dbContext.Database.BeginTransactionIfNotExistsAsync(cancellationToken);

        // Apply the block to the user
        await SetFriendStatus(fromUserId, toUserId, UserPartialFriendStatus.Blocked, cancellationToken);

        // Remove any pending or accepted friend requests from the other user
        await _dbContext.UserRelations.Where(x => x.FromUserId == toUserId && x.ToUserId == fromUserId && x.FriendStatus != UserPartialFriendStatus.Blocked)
            .ExecuteUpdateAsync(spc => spc.SetProperty(x => x.FriendStatus, UserPartialFriendStatus.None), cancellationToken);

        if (transaction is not null)
        {
            await transaction.CommitAsync(cancellationToken);
        }

        await _mediator.Publish(new UserBlockedEvent(fromUserId, toUserId), cancellationToken);

        return UpdateUserRelationResult.Success;
    }

    public async Task<UpdateUserRelationResult> UnblockUserAsync(Guid fromUserId, Guid toUserId, CancellationToken cancellationToken)
    {
        if (fromUserId == toUserId) return UpdateUserRelationResult.CannotApplyToSelf;

        // Apply the unblock to the user only if the user is blocked
        int numUpdates = await _dbContext.UserRelations.Where(x => x.FromUserId == fromUserId && x.ToUserId == toUserId && x.FriendStatus == UserPartialFriendStatus.Blocked)
            .ExecuteUpdateAsync(spc => spc.SetProperty(x => x.FriendStatus, UserPartialFriendStatus.None), cancellationToken);

        if (numUpdates <= 0)
        {
            return UpdateUserRelationResult.NoChanges;
        }

        await _mediator.Publish(new UserUnblockedEvent(fromUserId, toUserId), cancellationToken);

        return UpdateUserRelationResult.Success;
    }

    public async Task<UpdateUserRelationResult> SetUserRelationDetailsAsync(Guid fromUserId, Guid toUserId, SetUserRelationDto relationUpdate, CancellationToken cancellationToken)
    {
        if (fromUserId == toUserId) return UpdateUserRelationResult.CannotApplyToSelf;

        // Get all relations between the two users (if any)
        var relation = await _dbContext.UserRelations.AsTracking().FirstOrDefaultAsync(u => u.FromUserId == fromUserId && u.ToUserId == toUserId, cancellationToken);

        bool relationExists = true;
        if (relation is null)
        {
            relationExists = false;
            relation = new UserRelationEntity
            {
                FromUserId = fromUserId,
                ToUserId = toUserId
            };
        }

        if (relationUpdate.IsFavorite.HasValue)
        {
            relation.IsFavorite = relationUpdate.IsFavorite.Value;
        }
        if (relationUpdate.IsMuted.HasValue)
        {
            relation.IsMuted = relationUpdate.IsMuted.Value;
        }
        if (relationUpdate.NickName is not null)
        {
            relation.NickName = relationUpdate.NickName;
        }
        if (relationUpdate.Notes is not null)
        {
            relation.Notes = relationUpdate.Notes;
        }

        if (!relationExists)
        {
            await _dbContext.UserRelations.AddAsync(relation, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new UserRelationDetailsUpdatedEvent(fromUserId, toUserId));

        return UpdateUserRelationResult.Success;
    }

}
