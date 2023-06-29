using Microsoft.EntityFrameworkCore;
using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Enums;

namespace ZapMe.BusinessLogic.Users;

public static class FriendRequestLogic
{
    public static async Task<FriendRequestsDto> GetFriendRequestsDTO(this DatabaseContext dbContext, Guid userId, CancellationToken cancellationToken)
    {
        UserRelationEntity[] relevantUserRelations = await dbContext.UserRelations
            .Where(x => ((x.FromUserId == userId && x.ToUserId == userId) || (x.FromUserId == userId && x.ToUserId == userId)) && x.FriendStatus == UserFriendStatus.Pending)
            .ToArrayAsync(cancellationToken);

        return new FriendRequestsDto
        {
            Incoming = relevantUserRelations.Where(x => x.ToUserId == userId).Select(x => x.FromUserId).ToArray(),
            Outgoing = relevantUserRelations.Where(x => x.FromUserId == userId).Select(x => x.ToUserId).ToArray()
        };
    }
}
