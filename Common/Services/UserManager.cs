using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.Enums;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class UserManager : IUserManager
{
    private readonly DatabaseContext _dbContext;

    public UserManager(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CreateFriendRequestAsync(Guid sourceUserId, Guid targetUserId, CancellationToken cancellationToken)
    {
        // You can't send a friend request to yourself, that would be weird
        if (sourceUserId == targetUserId) return false;

        // Check if the users are already friends or blocked, if so, don't create a friend request
        if (await _dbContext
                .UserRelations
                .AnyAsync(x =>
                    (
                        (x.SourceUserId == sourceUserId && x.TargetUserId == targetUserId) ||
                        (x.SourceUserId == targetUserId && x.TargetUserId == sourceUserId)
                    )
                    &&
                    (
                        x.RelationType == UserRelationType.Friend ||
                        x.RelationType == UserRelationType.Blocked
                    ),
                    cancellationToken
                )
           )
        {
            return false;
        }

        // Get friend requests between the two users (if any)
        FriendRequestEntity[] friendRequests = await _dbContext
            .FriendRequests
            .Where(x => 
                (x.SenderId == sourceUserId && x.ReceiverId == targetUserId) ||
                (x.SenderId == targetUserId && x.ReceiverId == sourceUserId)
            )   
            .Take(2)
            .ToArrayAsync(cancellationToken);

        // If there's already a pending outgoing friend request, don't create a new one
        if (friendRequests.Any(x => x.SenderId == sourceUserId)) return false;

        // If there's a pending incoming friend request, accept it
        FriendRequestEntity? incomingRequest = friendRequests.FirstOrDefault(x => x.SenderId == targetUserId);
        if (incomingRequest is not null)
        {
            using IDbContextTransaction? transaction = await _dbContext.Database.BeginTransactionIfNotExistsAsync(cancellationToken);

            // Remove the incoming friend request
            _dbContext.FriendRequests.Remove(incomingRequest);

            // Set the users as friends
            await SetUserRelationTypeAsync(sourceUserId, targetUserId, UserRelationType.Friend, cancellationToken);

            if (transaction is not null) await transaction.CommitAsync(cancellationToken);

            return true;
        }

        // Create a new friend request
        _dbContext.FriendRequests.Add(new FriendRequestEntity()
        {
            SenderId = sourceUserId,
            ReceiverId = targetUserId
        });
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> AcceptFriendRequestAsync(Guid sourceUserId, Guid targetUserId, CancellationToken cancellationToken)
    {
        // You can't send a friend request to yourself, that would be weird
        if (sourceUserId == targetUserId) return false;

        FriendRequestEntity? incomingRequest = await _dbContext
            .FriendRequests
            .FirstOrDefaultAsync(x => x.SenderId == targetUserId && x.ReceiverId == sourceUserId, cancellationToken);

        if (incomingRequest is null) return false;

        using IDbContextTransaction? transaction = await _dbContext.Database.BeginTransactionIfNotExistsAsync(cancellationToken);

        _dbContext.FriendRequests.Remove(incomingRequest);
        await SetUserRelationTypeAsync(sourceUserId, targetUserId, UserRelationType.Friend, cancellationToken);

        if (transaction is not null) await transaction.CommitAsync(cancellationToken);

        return true;
    }

    public async Task<bool> RejectFriendRequestAsync(Guid sourceUserId, Guid targetUserId, CancellationToken cancellationToken)
    {
        // You can't send a friend request to yourself, that would be weird
        if (sourceUserId == targetUserId) return false;

        int nRemoved = await _dbContext
            .FriendRequests
            .Where(fr => fr.SenderId == targetUserId && fr.ReceiverId == sourceUserId)
            .ExecuteDeleteAsync(cancellationToken);
        return nRemoved > 0;
    }

    public async Task<bool> SetUserRelationTypeAsync(Guid sourceUserId, Guid targetUserId, UserRelationType relationType, CancellationToken cancellationToken)
    {
        // You can't have a relation with yourself, go love someone :D
        if (sourceUserId == targetUserId) return false;

        using IDbContextTransaction? transaction = await _dbContext.Database.BeginTransactionIfNotExistsAsync(cancellationToken);

        // Get the user relations between the two users (if any)
        UserRelationEntity[] relations = await _dbContext
            .UserRelations
            .AsTracking()
            .Where(x =>
                (x.SourceUserId == sourceUserId && x.TargetUserId == targetUserId) ||
                (x.SourceUserId == targetUserId && x.TargetUserId == sourceUserId)
            )
            .Take(2)
            .ToArrayAsync(cancellationToken);

        // If there's no relation from the source user to the target user, create one
        UserRelationEntity? sourceUserRelation = relations.FirstOrDefault(x => x.SourceUserId == sourceUserId);
        if (sourceUserRelation is not null)
        {
            sourceUserRelation.RelationType = relationType;
        }
        else if (relationType is not UserRelationType.None)
        {
            sourceUserRelation = new()
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId,
                RelationType = UserRelationType.None
            };
            _dbContext.UserRelations.Add(sourceUserRelation);
        }

        // If there's no relation from the target user to the source user, create one
        UserRelationEntity? targetUserRelation = relations.FirstOrDefault(x => x.SourceUserId == targetUserId);
        if (targetUserRelation is not null)
        {
            targetUserRelation.RelationType = relationType;
        }
        else if (relationType is UserRelationType.Friend)
        {
            targetUserRelation = new()
            {
                SourceUserId = targetUserId,
                TargetUserId = sourceUserId,
                RelationType = UserRelationType.Friend
            };
            _dbContext.UserRelations.Add(targetUserRelation);
        }

        // Ctrl + S :D
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (transaction is not null) await transaction.CommitAsync(cancellationToken);

        return true;
    }
}
