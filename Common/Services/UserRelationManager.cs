using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ZapMe.BusinessLogic.CQRS.Events;
using ZapMe.Database;
using ZapMe.Database.Extensions;
using ZapMe.Database.Models;
using ZapMe.DTOs;
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

    private Task<int> SetFriendStatus(Guid fromUserId, Guid toUserId, UserPartialRelationType friendStatus, UserRelationEntity? trackedEntity, CancellationToken cancellationToken)
    {
        if (trackedEntity is null)
        {
            trackedEntity = new UserRelationEntity
            {
                FromUserId = fromUserId,
                ToUserId = toUserId,
                FriendStatus = friendStatus,
            };

            _dbContext.UserRelations.Add(trackedEntity);
        }
        else
        {
            trackedEntity.FriendStatus = friendStatus;
        }

        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<int> SetFriendStatus(Guid fromUserId, Guid toUserId, UserPartialRelationType friendStatus, CancellationToken cancellationToken)
    {
        var trackedEntity = await _dbContext.UserRelations.AsTracking()
            .FirstOrDefaultAsync(x => x.FromUserId == fromUserId && x.ToUserId == toUserId, cancellationToken);

        return await SetFriendStatus(fromUserId, toUserId, friendStatus, trackedEntity, cancellationToken);
    }

    public async Task<CreateOrAcceptFriendRequestResult> CreateOrAcceptFriendRequestAsync(Guid fromUserId, Guid toUserId, CancellationToken cancellationToken)
    {
        if (fromUserId == toUserId) return CreateOrAcceptFriendRequestResult.CannotApplyToSelf;

        // Get all relations between the two users (if any)
        var relations = await _dbContext.UserRelations.AsTracking()
            .Where(x => (x.FromUserId == fromUserId && x.ToUserId == toUserId) || (x.FromUserId == toUserId && x.ToUserId == fromUserId))
            .Take(2).ToArrayAsync(cancellationToken);

        // Check if the users are already friends or blocked, if so, don't create a friend request
        if (relations.Any(x => x.FriendStatus == UserPartialRelationType.Blocked)) return CreateOrAcceptFriendRequestResult.NotAllowed;
        if (relations.Any(x => x.FriendStatus == UserPartialRelationType.Accepted)) return CreateOrAcceptFriendRequestResult.AlreadyFriends;

        // Seperate the relations into incoming and outgoing
        var outgoingRelation = relations.FirstOrDefault(x => x.FromUserId == fromUserId);
        var incomingRelation = relations.FirstOrDefault(x => x.FromUserId == toUserId);

        // If there's a pending incoming friend request, make the two users friends
        if (incomingRelation is not null && incomingRelation.FriendStatus == UserPartialRelationType.Pending)
        {
            using IDbContextTransaction? transaction = await _dbContext.Database.BeginTransactionIfNotExistsAsync(cancellationToken);

            // Force the friend status to accepted
            incomingRelation.FriendStatus = UserPartialRelationType.Accepted;
            await SetFriendStatus(fromUserId, toUserId, UserPartialRelationType.Accepted, outgoingRelation, cancellationToken);

            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            // Publish the friendship created event
            await _mediator.Publish(new UserRelationTypeChangedEvent(fromUserId, toUserId, UserRelationType.Friends), cancellationToken);
            await _mediator.Publish(new UserRelationTypeChangedEvent(toUserId, fromUserId, UserRelationType.Friends), cancellationToken);

            return CreateOrAcceptFriendRequestResult.FriendshipCreated;
        }

        // If there's a pending outgoing friend request, do nothing
        if (outgoingRelation is not null && outgoingRelation.FriendStatus == UserPartialRelationType.Pending) return CreateOrAcceptFriendRequestResult.NoChanges;

        // Set the friend status to pending
        await SetFriendStatus(fromUserId, toUserId, UserPartialRelationType.Pending, outgoingRelation, cancellationToken);

        // Publish the friend request created event
        await _mediator.Publish(new UserRelationTypeChangedEvent(fromUserId, toUserId, UserRelationType.FriendRequestSent), cancellationToken);
        await _mediator.Publish(new UserRelationTypeChangedEvent(toUserId, fromUserId, UserRelationType.FriendRequestReceived), cancellationToken);

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
                && fr.FriendStatus == UserPartialRelationType.Pending
            )
            .ExecuteUpdateAsync(spc => spc.SetProperty(x => x.FriendStatus, UserPartialRelationType.None), cancellationToken);

        if (nRemoved <= 0)
        {
            return UpdateUserRelationResult.NoChanges;
        }

        await _mediator.Publish(new UserRelationTypeChangedEvent(fromUserId, toUserId, UserRelationType.None), cancellationToken);
        await _mediator.Publish(new UserRelationTypeChangedEvent(toUserId, fromUserId, UserRelationType.None), cancellationToken);

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
                && (fr.FriendStatus == UserPartialRelationType.Pending || fr.FriendStatus == UserPartialRelationType.Accepted)
            )
            .ExecuteUpdateAsync(spc => spc.SetProperty(x => x.FriendStatus, UserPartialRelationType.None), cancellationToken);

        if (nRemoved <= 0)
        {
            return UpdateUserRelationResult.NoChanges;
        }

        await _mediator.Publish(new UserRelationTypeChangedEvent(fromUserId, toUserId, UserRelationType.None), cancellationToken);
        await _mediator.Publish(new UserRelationTypeChangedEvent(toUserId, fromUserId, UserRelationType.None), cancellationToken);

        return UpdateUserRelationResult.Success;
    }

    public async Task<UpdateUserRelationResult> BlockUserAsync(Guid fromUserId, Guid toUserId, CancellationToken cancellationToken)
    {
        if (fromUserId == toUserId) return UpdateUserRelationResult.CannotApplyToSelf;

        using IDbContextTransaction? transaction = await _dbContext.Database.BeginTransactionIfNotExistsAsync(cancellationToken);

        // Apply the block to the user
        int outChanges = await SetFriendStatus(fromUserId, toUserId, UserPartialRelationType.Blocked, cancellationToken);

        // Remove any pending or accepted friend requests from the other user
        int inChanges = await _dbContext.UserRelations
            .Where(x => x.FromUserId == toUserId && x.ToUserId == fromUserId && x.FriendStatus != UserPartialRelationType.Blocked)
            .ExecuteUpdateAsync(spc => spc.SetProperty(x => x.FriendStatus, UserPartialRelationType.None), cancellationToken);

        if (transaction is not null)
        {
            await transaction.CommitAsync(cancellationToken);
        }

        if (outChanges <= 0 && inChanges <= 0)
        {
            return UpdateUserRelationResult.NoChanges;
        }

        // Send user blocked event if the user was blocked
        if (outChanges > 0)
        {
            await _mediator.Publish(new UserRelationTypeChangedEvent(fromUserId, toUserId, UserRelationType.Blocked), cancellationToken);
        }

        // Send user no longer friend event if the user was a friend
        if (inChanges > 0)
        {
            await _mediator.Publish(new UserRelationTypeChangedEvent(toUserId, fromUserId, UserRelationType.None), cancellationToken);
        }

        return UpdateUserRelationResult.Success;
    }

    public async Task<UpdateUserRelationResult> UnblockUserAsync(Guid fromUserId, Guid toUserId, CancellationToken cancellationToken)
    {
        if (fromUserId == toUserId) return UpdateUserRelationResult.CannotApplyToSelf;

        // Apply the unblock to the user only if the user is blocked
        int numUpdates = await _dbContext.UserRelations.Where(x => x.FromUserId == fromUserId && x.ToUserId == toUserId && x.FriendStatus == UserPartialRelationType.Blocked)
            .ExecuteUpdateAsync(spc => spc.SetProperty(x => x.FriendStatus, UserPartialRelationType.None), cancellationToken);

        if (numUpdates <= 0)
        {
            return UpdateUserRelationResult.NoChanges;
        }

        await _mediator.Publish(new UserRelationTypeChangedEvent(fromUserId, toUserId, UserRelationType.None), cancellationToken);

        return UpdateUserRelationResult.Success;
    }

    public async Task<UpdateUserRelationResult> SetUserRelationDetailsAsync(Guid fromUserId, Guid toUserId, UserRelationUpdateDto relationUpdate, CancellationToken cancellationToken)
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

        int nChanges = await _dbContext.SaveChangesAsync(cancellationToken);

        if (nChanges <= 0)
        {
            return UpdateUserRelationResult.NoChanges;
        }

        await _mediator.Publish(new UserRelationDetailsUpdatedEvent(fromUserId, toUserId, relation.IsFavorite, relation.IsMuted, relationUpdate.NickName, relationUpdate.Notes));

        return UpdateUserRelationResult.Success;
    }

}
