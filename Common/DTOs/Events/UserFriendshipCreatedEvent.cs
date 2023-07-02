namespace ZapMe.DTOs.Mediator;

public record struct UserFriendshipCreatedEvent(Guid FromUserId, Guid ToUserId);