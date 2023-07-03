namespace ZapMe.BusinessLogic.CQRS.Events;

public record struct UserRelationDetailsUpdatedEvent(Guid FromUserId, Guid ToUserId);