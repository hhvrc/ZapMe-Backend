using Mediator;

namespace ZapMe.BusinessLogic.CQRS.Events;

public record struct UserFriendshipDeletedEvent(Guid FromUserId, Guid ToUserId) : INotification;