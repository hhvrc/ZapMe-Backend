namespace ZapMe.DTOs.Mediator;

public record struct UserUnblockedEvent(Guid FromUserId, Guid ToUserId);