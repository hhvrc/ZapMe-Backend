using Mediator;

namespace ZapMe.BusinessLogic.CQRS.Events;

public readonly record struct UserOfflineEvent(Guid UserId) : INotification;