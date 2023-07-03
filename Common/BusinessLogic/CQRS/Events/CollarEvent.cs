using Mediator;

namespace ZapMe.BusinessLogic.CQRS.Events;

public readonly record struct CollarEvent(Guid FromUserId, Guid ToUserId) : INotification;