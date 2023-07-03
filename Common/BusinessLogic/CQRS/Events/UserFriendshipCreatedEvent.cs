namespace ZapMe.BusinessLogic.CQRS.Events;

public record struct UserFriendshipCreatedEvent(Guid FromUserId, Guid ToUserId);