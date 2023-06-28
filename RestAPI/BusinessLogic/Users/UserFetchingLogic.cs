using Microsoft.EntityFrameworkCore;
using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Enums;

namespace ZapMe.BusinessLogic.Users;

public static class UserFetchingLogic
{
    private static IQueryable<UserEntity> DatabaseQueryBase(DatabaseContext dbContext, bool tracking = false)
    {
        return tracking
            ? dbContext.Users.AsTracking()
            : dbContext.Users.AsNoTracking();
    }
    private static IQueryable<UserEntity> DatabaseQueryWithRelations(DatabaseContext dbContext, bool tracking = false)
    {
        return DatabaseQueryBase(dbContext, tracking)
            .Include(u => u.RelationsIncoming)
            .Include(u => u.RelationsOutgoing);
    }

    /// <summary>
    /// Made to fetch data for the <see cref="AccountDto"/> object
    /// </summary>
    private static IQueryable<UserEntity> DatabaseQueryForAccountDto(DatabaseContext dbContext, bool tracking = false)
    {
        return DatabaseQueryBase(dbContext, tracking)
            .Include(u => u.ProfileAvatar)
            .Include(u => u.ProfileBanner)
            .Include(u => u.UserRoles)
            .Include(u => u.RelationsOutgoing)
            .Include(u => u.FriendRequestsOutgoing)
            .Include(u => u.FriendRequestsIncoming)
            .Include(u => u.SSOConnections);
    }

    public static async Task<AccountDto> FetchAccountDto_ById(DatabaseContext dbContext, Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await DatabaseQueryForAccountDto(dbContext).FirstAsync(x => x.Id == userId, cancellationToken);

        return UserMapper.ToAccountDto(user);
    }

    public static async Task<UserDto?> FetchUserDto_AsUser_ById(DatabaseContext dbContext, Guid requestingUserId, Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await DatabaseQueryWithRelations(dbContext).FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null) return null;

        // Get the incoming and outgoing relations for the requesting user
        UserRelationEntity? relation = user.RelationsIncoming.FirstOrDefault(r => r.SourceUserId == requestingUserId);
        UserRelationEntity? foreignRelation = user.RelationsOutgoing.FirstOrDefault(r => r.TargetUserId == requestingUserId);

        // Check if the user is blocked by the requesting user or vice versa
        bool blockActive = relation?.RelationType == UserRelationType.Blocked || foreignRelation?.RelationType == UserRelationType.Blocked;

        // Return the user (minimal if a block is active)
        return blockActive ?
            UserMapper.ToMinimalUserDto(user, relation) :
            UserMapper.ToUserDto(user, relation);
    }

    public static async Task<UserDto?> FetchUserDto_AsUser_ByName(DatabaseContext dbContext, Guid requestingUserId, string userName, CancellationToken cancellationToken = default)
    {
        var user = await DatabaseQueryWithRelations(dbContext).FirstOrDefaultAsync(x => x.Name == userName, cancellationToken);

        if (user is null) return null;

        // Get the incoming and outgoing relations for the requesting user
        UserRelationEntity? relation = user.RelationsIncoming.FirstOrDefault(r => r.SourceUserId == requestingUserId);
        UserRelationEntity? foreignRelation = user.RelationsOutgoing.FirstOrDefault(r => r.TargetUserId == requestingUserId);

        // Check if the user is blocked by the requesting user or vice versa
        bool blockActive = relation?.RelationType == UserRelationType.Blocked || foreignRelation?.RelationType == UserRelationType.Blocked;

        // Return the user (minimal if a block is active)
        return blockActive ?
            UserMapper.ToMinimalUserDto(user, relation) :
            UserMapper.ToUserDto(user, relation);
    }
}