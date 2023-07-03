using Mediator;

namespace ZapMe.BusinessLogic.CQRS.Events;

public readonly record struct UserRelationDetailsUpdatedEvent(Guid FromUserId, Guid ToUserId) : INotification;