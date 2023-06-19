using ZapMe.Database.Models;

namespace ZapMe.DTOs;

public static class UserRelationMapper
{
    public static FriendDto ToFriendDto(this UserRelationEntity userRelation)
    {
        return new FriendDto
        {
            FriendId = userRelation.TargetUserId,
            NickName = userRelation.NickName,
            Notes = userRelation.Notes,
            FriendedAt = userRelation.CreatedAt
        };
    }
}
