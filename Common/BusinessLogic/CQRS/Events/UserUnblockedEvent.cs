using Mediator;

namespace ZapMe.BusinessLogic.CQRS.Events;

public record struct UserUnblockedEvent(Guid FromUserId, Guid ToUserId) : INotification;