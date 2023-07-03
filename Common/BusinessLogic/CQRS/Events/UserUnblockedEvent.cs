using Mediator;

namespace ZapMe.BusinessLogic.CQRS.Events;

public readonly record struct UserUnblockedEvent(Guid FromUserId, Guid ToUserId) : INotification;