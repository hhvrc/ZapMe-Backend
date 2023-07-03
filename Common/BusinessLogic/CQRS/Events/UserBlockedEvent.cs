namespace ZapMe.BusinessLogic.CQRS.Events;

public record struct UserBlockedEvent(Guid FromUserId, Guid ToUserId);