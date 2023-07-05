using fbs.server;
using Mediator;
using ZapMe.BusinessLogic.CQRS.Events;
using ZapMe.Websocket;

namespace ZapMe.BusinessLogic.CQRS.Handlers;

public sealed class UserRelationTypeChangedEventHandler : INotificationHandler<UserRelationTypeChangedEvent>
{
    public async ValueTask Handle(UserRelationTypeChangedEvent notification, CancellationToken cancellationToken)
    {
        await WebSocketHub.RunActionOnUserAsync(notification.FromUserId, (client) => client.SendPayloadAsync(new ServerPayload(new UserRelationTypeChanged
        {
            UserId = notification.ToUserId.ToString(),
            RelationType = notification.FriendStatus switch
            {
                Enums.UserRelationType.None => UserRelationType.none,
                Enums.UserRelationType.FriendRequestSent => UserRelationType.friend_request_sent,
                Enums.UserRelationType.FriendRequestReceived => UserRelationType.friend_request_received,
                Enums.UserRelationType.Friends => UserRelationType.friend,
                Enums.UserRelationType.Blocked => UserRelationType.blocked,
                _ => throw new NotImplementedException()
            }
        }), cancellationToken));
    }
}
