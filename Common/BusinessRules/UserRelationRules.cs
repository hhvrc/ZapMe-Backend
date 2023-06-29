using ZapMe.Database.Models;
using ZapMe.Enums;

namespace ZapMe.BusinessRules;

public static class UserRelationRules
{
    public static bool IsEitherUserBlocking(UserEntity userA, Guid userBId)
    {
        return userA.RelationsOutgoing.Any(fs => fs.ToUserId == userBId && fs.FriendStatus == UserFriendStatus.Blocked)
            || userA.RelationsIncoming.Any(fs => fs.FromUserId == userBId && fs.FriendStatus == UserFriendStatus.Blocked);
    }
}
