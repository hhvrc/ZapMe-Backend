using fbs.server;
using Mediator;
using Microsoft.Extensions.Logging;
using ZapMe.BusinessLogic.CQRS.Events;
using ZapMe.Websocket;

namespace ZapMe.BusinessLogic.CQRS.Handlers;

public sealed class UserFriendRequestEventHandler : INotificationHandler<UserFriendRequestCreatedEvent>, INotificationHandler<UserFriendRequestRemovedEvent>
{
    private readonly ILogger<UserFriendRequestEventHandler> _logger;

    public UserFriendRequestEventHandler(ILogger<UserFriendRequestEventHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask Handle(UserFriendRequestCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UserFriendRequestCreatedEvent: {fromUserId} -> {toUserId}", notification.FromUserId, notification.ToUserId);

        var payload = new ServerPayload(new FriendRequestAdded
        {
            SenderUserId = notification.FromUserId.ToString(),
            ReceiverUserId = notification.ToUserId.ToString()
        });

        await Task.WhenAll(
            WebSocketHub.RunActionOnUserAsync(notification.FromUserId, (client) => client.SendPayloadAsync(payload, cancellationToken)),
            WebSocketHub.RunActionOnUserAsync(notification.ToUserId, (client) => client.SendPayloadAsync(payload, cancellationToken))
            );
    }

    public async ValueTask Handle(UserFriendRequestRemovedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UserFriendRequestRemovedEvent: {fromUserId} -> {toUserId}", notification.FromUserId, notification.ToUserId);

        var payload = new ServerPayload(new FriendRequestRemoved
        {
            SenderUserId = notification.FromUserId.ToString(),
            ReceiverUserId = notification.ToUserId.ToString()
        });

        await Task.WhenAll(
            WebSocketHub.RunActionOnUserAsync(notification.FromUserId, (client) => client.SendPayloadAsync(payload, cancellationToken)),
            WebSocketHub.RunActionOnUserAsync(notification.ToUserId, (client) => client.SendPayloadAsync(payload, cancellationToken))
            );
    }
}
