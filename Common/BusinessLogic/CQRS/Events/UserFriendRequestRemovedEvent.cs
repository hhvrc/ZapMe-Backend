using Mediator;

namespace ZapMe.BusinessLogic.CQRS.Events;

public readonly record struct UserFriendRequestRemovedEvent(Guid FromUserId, Guid ToUserId) : INotification;