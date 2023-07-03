namespace ZapMe.BusinessLogic.CQRS.Events;

public record struct UserFriendRequestCreatedEvent(Guid FromUserId, Guid ToUserId);