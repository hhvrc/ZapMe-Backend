namespace ZapMe.DTOs.Mediator;

public record struct UserBlockedEvent(Guid FromUserId, Guid ToUserId);