using Mediator;

namespace ZapMe.BusinessLogic.CQRS.Events;

public record struct UserFriendRequestDeletedEvent(Guid FromUserId, Guid ToUserId) : INotification;