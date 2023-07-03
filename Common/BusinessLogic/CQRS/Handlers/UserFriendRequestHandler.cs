using Mediator;
using Microsoft.Extensions.Logging;
using ZapMe.BusinessLogic.CQRS.Events;
using ZapMe.BusinessLogic.Serialization.Flatbuffers;
using ZapMe.Services.Interfaces;

namespace ZapMe.BusinessLogic.CQRS.Handlers;

public sealed class UserFriendRequestHandler : INotificationHandler<UserFriendRequestCreatedEvent>, INotificationHandler<UserFriendRequestRemovedEvent>
{
    private readonly IWebSocketClientHub _wsClientHub;
    private readonly ILogger<UserFriendRequestHandler> _logger;

    public UserFriendRequestHandler(IWebSocketClientHub wsClientHub, ILogger<UserFriendRequestHandler> logger)
    {
        _wsClientHub = wsClientHub;
        _logger = logger;
    }

    public async ValueTask Handle(UserFriendRequestCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"UserFriendRequestCreatedEvent: {notification.FromUserId} -> {notification.ToUserId}");

        await UserFriendRequestSerializer.SerializeAdded(notification.FromUserId, notification.ToUserId, (bytes) => Task.WhenAll(
            _wsClientHub.RunActionOnUserClientsAsync(notification.FromUserId, (client) => client.SendMessageAsync(bytes, cancellationToken), cancellationToken),
            _wsClientHub.RunActionOnUserClientsAsync(notification.ToUserId, (client) => client.SendMessageAsync(bytes, cancellationToken), cancellationToken)
        ));
    }

    public async ValueTask Handle(UserFriendRequestRemovedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"UserFriendRequestRemovedEvent: {notification.FromUserId} -> {notification.ToUserId}");

        await UserFriendRequestSerializer.SerializeRemoved(notification.FromUserId, notification.ToUserId, (bytes) => Task.WhenAll(
            _wsClientHub.RunActionOnUserClientsAsync(notification.FromUserId, (client) => client.SendMessageAsync(bytes, cancellationToken), cancellationToken),
            _wsClientHub.RunActionOnUserClientsAsync(notification.ToUserId, (client) => client.SendMessageAsync(bytes, cancellationToken), cancellationToken)
        ));
    }
}
