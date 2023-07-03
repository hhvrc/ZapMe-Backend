namespace ZapMe.BusinessLogic.CQRS.Events;

public record struct CollarEvent(Guid FromUserId, Guid ToUserId);