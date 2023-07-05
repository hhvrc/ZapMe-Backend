using Mediator;
using ZapMe.Enums;

namespace ZapMe.BusinessLogic.CQRS.Events;

public readonly record struct UserRelationTypeChangedEvent(Guid FromUserId, Guid ToUserId, UserRelationType FriendStatus) : INotification;