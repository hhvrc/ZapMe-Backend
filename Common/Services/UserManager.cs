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

    public Task<UserEntity?> GetByIdAsync(Guid requestingUserId, Guid userId, CancellationToken cancellationToken = default)
    {
        if (requestingUserId == userId)
        {
            // Get the user
            return _dbContext
                .Users
                .AsNoTracking()
                .Include(u => u.RelationsIncoming)
                .Include(u => u.RelationsOutgoing)
                .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        }

        // Get the user
        return _dbContext
            .Users
            .AsNoTracking()
            .Include(u => u.RelationsIncoming)
            .Include(u => u.RelationsOutgoing)
            .FirstOrDefaultAsync(x =>
                (x.Id == userId) &&
                (
                    !x.RelationsOutgoing.Any(r => r.TargetUserId == requestingUserId && r.RelationType == UserRelationType.Blocked) &&
                    !x.RelationsIncoming.Any(r => r.SourceUserId == requestingUserId && r.RelationType == UserRelationType.Blocked)
                ), cancellationToken);
    }

    public Task<UserEntity?> GetByUserNameAsync(Guid requestingUserId, string userName, CancellationToken cancellationToken = default)
    {
        // Get the user
        return _dbContext
            .Users
            .AsNoTracking()
            .Include(u => u.RelationsIncoming)
            .Include(u => u.RelationsOutgoing)
            .FirstOrDefaultAsync(x =>
                (x.Name == userName) &&
                (
                    !x.RelationsOutgoing.Any(r => r.TargetUserId == requestingUserId && r.RelationType == UserRelationType.Blocked) &&
                    !x.RelationsIncoming.Any(r => r.SourceUserId == requestingUserId && r.RelationType == UserRelationType.Blocked)
                ), cancellationToken);
    }

    public async Task<bool> CreateFriendRequestAsync(Guid requestingUserId, Guid targetUserId, CancellationToken cancellationToken)
    {
        // You can't send a friend request to yourself, that would be weird
        if (requestingUserId == targetUserId) return false;

        // Check if the users are already friends or blocked, if so, don't create a friend request
        if (await _dbContext
                .UserRelations
                .AnyAsync(x =>
                    (
                        (x.SourceUserId == requestingUserId && x.TargetUserId == targetUserId) ||
                        (x.SourceUserId == targetUserId && x.TargetUserId == requestingUserId)
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
                (x.SenderId == requestingUserId && x.ReceiverId == targetUserId) ||
                (x.SenderId == targetUserId && x.ReceiverId == requestingUserId)
            )
            .Take(2)
            .ToArrayAsync(cancellationToken);

        // If there's already a pending outgoing friend request, don't create a new one
        if (friendRequests.Any(x => x.SenderId == requestingUserId)) return false;

        // If there's a pending incoming friend request, accept it
        FriendRequestEntity? incomingRequest = friendRequests.FirstOrDefault(x => x.SenderId == targetUserId);
        if (incomingRequest is not null)
        {
            using IDbContextTransaction? transaction = await _dbContext.Database.BeginTransactionIfNotExistsAsync(cancellationToken);

            // Remove any pending friend requests
            await DeleteFriendRequestAsync(targetUserId, requestingUserId, cancellationToken);

            // Set the users as friends
            await SetUserRelationTypeAsync(requestingUserId, targetUserId, UserRelationType.Friend, cancellationToken);

            if (transaction is not null) await transaction.CommitAsync(cancellationToken);

            return true;
        }

        // Create a new friend request
        _dbContext.FriendRequests.Add(new FriendRequestEntity()
        {
            SenderId = requestingUserId,
            ReceiverId = targetUserId
        });
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> AcceptFriendRequestAsync(Guid requestingUserId, Guid targetUserId, CancellationToken cancellationToken)
    {
        // You can't send a friend request to yourself, that would be weird
        if (requestingUserId == targetUserId) return false;

        bool hasIncoming = await _dbContext
            .FriendRequests
            .AnyAsync(x => x.SenderId == targetUserId && x.ReceiverId == requestingUserId, cancellationToken);

        if (!hasIncoming) return false;

        using IDbContextTransaction? transaction = await _dbContext.Database.BeginTransactionIfNotExistsAsync(cancellationToken);

        await DeleteFriendRequestAsync(requestingUserId, targetUserId, cancellationToken);
        await SetUserRelationTypeAsync(requestingUserId, targetUserId, UserRelationType.Friend, cancellationToken);

        if (transaction is not null) await transaction.CommitAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteFriendRequestAsync(Guid requestingUserId, Guid targetUserId, CancellationToken cancellationToken)
    {
        // You can't send a friend request to yourself, that would be weird
        if (requestingUserId == targetUserId) return false;

        int nRemoved = await _dbContext
            .FriendRequests
            .Where(fr =>
                (fr.SenderId == targetUserId && fr.ReceiverId == requestingUserId) ||
                (fr.SenderId == requestingUserId && fr.ReceiverId == targetUserId)
            )
            .ExecuteDeleteAsync(cancellationToken);

        return nRemoved > 0;
    }

    public async Task<bool> SetUserRelationTypeAsync(Guid requestingUserId, Guid targetUserId, UserRelationType relationType, CancellationToken cancellationToken)
    {
        // You can't have a relation with yourself, go love someone :D
        if (requestingUserId == targetUserId) return false;

        using IDbContextTransaction? transaction = await _dbContext.Database.BeginTransactionIfNotExistsAsync(cancellationToken);

        // Get the user relations between the two users (if any)
        UserRelationEntity[] relations = await _dbContext
            .UserRelations
            .AsTracking()
            .Where(x =>
                (x.SourceUserId == requestingUserId && x.TargetUserId == targetUserId) ||
                (x.SourceUserId == targetUserId && x.TargetUserId == requestingUserId)
            )
            .Take(2)
            .ToArrayAsync(cancellationToken);

        // If there's no relation from the source user to the target user, create one
        UserRelationEntity? requestingUserRelation = relations.FirstOrDefault(x => x.SourceUserId == requestingUserId);
        if (requestingUserRelation is not null)
        {
            requestingUserRelation.RelationType = relationType;
        }
        else if (relationType is not UserRelationType.None)
        {
            requestingUserRelation = new()
            {
                SourceUserId = requestingUserId,
                TargetUserId = targetUserId,
                RelationType = relationType
            };
            _dbContext.UserRelations.Add(requestingUserRelation);
        }

        // If there's no relation from the target user to the source user, create one
        UserRelationEntity? targetUserRelation = relations.FirstOrDefault(x => x.SourceUserId == targetUserId);
        if (targetUserRelation is not null)
        {
            // If we are friending the users, set the relation type to friend
            if (relationType is UserRelationType.Friend)
            {
                targetUserRelation.RelationType = UserRelationType.Friend;
            }
            // Else if the relation is already friend, set it to none
            else if (targetUserRelation.RelationType is UserRelationType.Friend)
            {
                targetUserRelation.RelationType = UserRelationType.None;
            }
        }
        else if (relationType is UserRelationType.Friend)
        {
            targetUserRelation = new()
            {
                SourceUserId = targetUserId,
                TargetUserId = requestingUserId,
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
