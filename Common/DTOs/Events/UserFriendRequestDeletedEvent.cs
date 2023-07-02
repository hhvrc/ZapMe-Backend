namespace ZapMe.DTOs.Mediator;

public record struct UserFriendRequestDeletedEvent(Guid FromUserId, Guid ToUserId);