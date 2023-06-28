using Microsoft.EntityFrameworkCore;
using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.DTOs;

namespace ZapMe.BusinessLogic.Users;

public static class FriendRequestLogic
{
    public static async Task<FriendRequestsDTO> GetFriendRequestsDTO(this DatabaseContext dbContext, Guid userId, CancellationToken cancellationToken)
    {
        FriendRequestEntity[] friendRequests = await dbContext
            .FriendRequests
            .Where(x => x.SenderId == userId || x.ReceiverId == userId)
            .ToArrayAsync(cancellationToken);

        return new FriendRequestsDTO
        {
            Incoming = friendRequests.Where(x => x.ReceiverId == userId).Select(x => x.SenderId).ToArray(),
            Outgoing = friendRequests.Where(x => x.SenderId == userId).Select(x => x.ReceiverId).ToArray(),
        };
    }
}
