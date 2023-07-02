namespace ZapMe.DTOs.Mediator;

public record struct UserRelationDetailsUpdatedEvent(Guid FromUserId, Guid ToUserId);