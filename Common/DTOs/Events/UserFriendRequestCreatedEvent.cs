namespace ZapMe.DTOs.Mediator;

public record struct UserFriendRequestCreatedEvent(Guid FromUserId, Guid ToUserId);