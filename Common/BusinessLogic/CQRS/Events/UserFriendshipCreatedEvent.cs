using Mediator;

namespace ZapMe.BusinessLogic.CQRS.Events;

public readonly record struct UserFriendshipCreatedEvent(Guid FromUserId, Guid ToUserId) : INotification;