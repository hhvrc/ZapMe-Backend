using ZapMe.Database.Models;
using ZapMe.Enums;

namespace ZapMe.BusinessRules;

public static class UserRelationRules
{
    public static bool IsEitherUserBlocking(UserEntity fromUser, Guid toUserId)
    {
        return fromUser.RelationsOutgoing.Any(r => r.ToUserId == toUserId && r.FriendStatus == UserFriendStatus.Blocked)
            || fromUser.RelationsIncoming.Any(r => r.FromUserId == toUserId && r.FriendStatus == UserFriendStatus.Blocked);
    }
}
