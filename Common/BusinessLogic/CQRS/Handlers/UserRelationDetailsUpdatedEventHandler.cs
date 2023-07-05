using fbs.server;
using Mediator;
using ZapMe.BusinessLogic.CQRS.Events;
using ZapMe.Websocket;

namespace ZapMe.BusinessLogic.CQRS.Handlers;

public sealed class UserRelationDetailsUpdatedEventHandler : INotificationHandler<UserRelationDetailsUpdatedEvent>
{
    public async ValueTask Handle(UserRelationDetailsUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await WebSocketHub.RunActionOnUserAsync(notification.FromUserId, (client) => client.SendPayloadAsync(new ServerPayload(new UserRelationDetailsUpdated
        {
            UserId = notification.ToUserId.ToString(),
            IsFavorite = notification.IsFavorite,
            IsMuted = notification.IsMuted,
            Nickname = notification.Nickname,
            Notes = notification.Notes
        }), cancellationToken));
    }
}
