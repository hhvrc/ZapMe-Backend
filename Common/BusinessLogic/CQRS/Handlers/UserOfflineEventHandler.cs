using fbs.server;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZapMe.BusinessLogic.CQRS.Events;
using ZapMe.Database;
using ZapMe.Websocket;

namespace ZapMe.BusinessLogic.CQRS.Handlers;

public sealed class UserOfflineEventHandler : INotificationHandler<UserOfflineEvent>
{
    private readonly DatabaseContext _dbContext;

    public UserOfflineEventHandler(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask Handle(UserOfflineEvent notification, CancellationToken cancellationToken)
    {
        var relations = await _dbContext.UserRelations.Where(r => r.ToUserId == notification.UserId && r.FriendStatus == Enums.UserPartialRelationType.Accepted).ToArrayAsync(cancellationToken);

        foreach (var relation in relations)
        {
            await WebSocketHub.RunActionOnUserAsync(relation.FromUserId, (client) => client.SendPayloadAsync(new ServerPayload(new UserOnlineStatusChanged
            {
                UserId = notification.UserId.ToString(),
                OnlineStatus = UserOnlineStatus.offline
            }), cancellationToken));
        }
    }
}
