using Microsoft.EntityFrameworkCore;
using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Enums;

namespace ZapMe.BusinessLogic.Users;

public static class UserRelationLogic
{
    public static async Task<FriendRequestsDto> GetFriendRequestsDTO(this DatabaseContext dbContext, Guid userId, CancellationToken cancellationToken)
    {
        UserRelationEntity[] relevantUserRelations = await dbContext.UserRelations
            .Where(x => (x.FromUserId == userId || x.ToUserId == userId) && x.FriendStatus == UserPartialRelationType.Pending)
            .ToArrayAsync(cancellationToken);

        return new FriendRequestsDto
        {
            Incoming = relevantUserRelations.Where(x => x.ToUserId == userId).Select(x => x.FromUserId).ToArray(),
            Outgoing = relevantUserRelations.Where(x => x.FromUserId == userId).Select(x => x.ToUserId).ToArray()
        };
    }

    public static UserRelationType GetUserRelationType(UserRelationEntity? outgoingRelation, UserRelationEntity? incomingRelation)
    {
        var incomingStatus = incomingRelation?.FriendStatus == UserPartialRelationType.Pending ? UserRelationType.FriendRequestReceived : UserRelationType.None;

        if (outgoingRelation == null) return incomingStatus;

        return outgoingRelation.FriendStatus switch
        {
            UserPartialRelationType.None => incomingStatus,
            UserPartialRelationType.Pending => UserRelationType.FriendRequestSent,
            UserPartialRelationType.Accepted => UserRelationType.Friends,
            UserPartialRelationType.Blocked => UserRelationType.Blocked,
            _ => throw new ArgumentOutOfRangeException(nameof(outgoingRelation.FriendStatus), outgoingRelation.FriendStatus, null)
        };
    }

    public static UserRelationDto GetUserRelationDto(Guid fromUser, UserEntity toUser)
    {
        var outgoingRelation = toUser.RelationsIncoming.FirstOrDefault(r => r.FromUserId == fromUser);
        var incomingRelation = toUser.RelationsOutgoing.FirstOrDefault(r => r.ToUserId == fromUser);

        return new UserRelationDto
        {
            Type = GetUserRelationType(outgoingRelation, incomingRelation),
            IsFavorite = outgoingRelation?.IsFavorite ?? false,
            IsMuted = outgoingRelation?.IsMuted ?? false,
            NickName = outgoingRelation?.NickName,
            Notes = outgoingRelation?.Notes,
            FriendedAt = outgoingRelation?.CreatedAt
        };
    }
}
