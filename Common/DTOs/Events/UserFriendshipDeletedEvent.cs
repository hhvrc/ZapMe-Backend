namespace ZapMe.DTOs.Mediator;

public record struct UserFriendshipDeletedEvent(Guid FromUserId, Guid ToUserId);