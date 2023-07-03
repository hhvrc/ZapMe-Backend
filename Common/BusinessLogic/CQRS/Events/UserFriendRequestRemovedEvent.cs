using Mediator;

namespace ZapMe.BusinessLogic.CQRS.Events;

public record struct UserFriendRequestRemovedEvent(Guid FromUserId, Guid ToUserId) : INotification;