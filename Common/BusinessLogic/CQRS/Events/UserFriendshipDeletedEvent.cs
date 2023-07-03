using Mediator;

namespace ZapMe.BusinessLogic.CQRS.Events;

public readonly record struct UserFriendshipDeletedEvent(Guid FromUserId, Guid ToUserId) : INotification;