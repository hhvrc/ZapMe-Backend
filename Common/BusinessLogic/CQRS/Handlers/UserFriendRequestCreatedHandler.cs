using Mediator;
using Microsoft.Extensions.Logging;
using ZapMe.BusinessLogic.CQRS.Events;
using ZapMe.BusinessLogic.Serialization.Flatbuffers;
using ZapMe.Services.Interfaces;

namespace ZapMe.BusinessLogic.CQRS.Handlers;

public sealed class UserFriendRequestCreatedHandler : INotificationHandler<UserFriendRequestCreatedEvent>
{
    private readonly IWebSocketClientHub _wsClientHub;
    private readonly ILogger<UserFriendRequestCreatedHandler> _logger;

    public UserFriendRequestCreatedHandler(IWebSocketClientHub wsClientHub, ILogger<UserFriendRequestCreatedHandler> logger)
    {
        _wsClientHub = wsClientHub;
        _logger = logger;
    }

    public async ValueTask Handle(UserFriendRequestCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"UserFriendRequestCreatedEvent: {notification.FromUserId} -> {notification.ToUserId}");

        await UserFriendRequestSerializer.Serialize(notification.FromUserId, notification.ToUserId, (bytes) =>
            _wsClientHub.RunActionOnUserClientsAsync(notification.ToUserId, (client) =>
                client.SendMessageAsync(bytes, cancellationToken)
                , cancellationToken)
        );
    }
}
