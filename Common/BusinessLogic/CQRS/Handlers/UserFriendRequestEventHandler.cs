using Mediator;
using Microsoft.Extensions.Logging;
using ZapMe.BusinessLogic.CQRS.Events;
using ZapMe.BusinessLogic.Serialization.Flatbuffers;
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
        _logger.LogInformation($"UserFriendRequestCreatedEvent: {notification.FromUserId} -> {notification.ToUserId}");

        await UserFriendRequestSerializer.SerializeAdded(notification.FromUserId, notification.ToUserId, (bytes) => Task.WhenAll(
            WebSocketHub.RunActionOnUserAsync(notification.FromUserId, (client) => client.SendMessageAsync(bytes, cancellationToken)),
            WebSocketHub.RunActionOnUserAsync(notification.ToUserId, (client) => client.SendMessageAsync(bytes, cancellationToken))
        ));
    }

    public async ValueTask Handle(UserFriendRequestRemovedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"UserFriendRequestRemovedEvent: {notification.FromUserId} -> {notification.ToUserId}");

        await UserFriendRequestSerializer.SerializeRemoved(notification.FromUserId, notification.ToUserId, (bytes) => Task.WhenAll(
            WebSocketHub.RunActionOnUserAsync(notification.FromUserId, (client) => client.SendMessageAsync(bytes, cancellationToken)),
            WebSocketHub.RunActionOnUserAsync(notification.ToUserId, (client) => client.SendMessageAsync(bytes, cancellationToken))
        ));
    }
}
