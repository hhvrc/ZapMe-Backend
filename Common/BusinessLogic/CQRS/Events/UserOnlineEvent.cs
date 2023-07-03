using Mediator;

namespace ZapMe.BusinessLogic.CQRS.Events;

public readonly record struct UserOnlineEvent(Guid UserId) : INotification;