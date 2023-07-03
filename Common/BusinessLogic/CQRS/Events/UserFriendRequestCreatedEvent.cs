using Mediator;

namespace ZapMe.BusinessLogic.CQRS.Events;

public readonly record struct UserFriendRequestCreatedEvent(Guid FromUserId, Guid ToUserId) : INotification;