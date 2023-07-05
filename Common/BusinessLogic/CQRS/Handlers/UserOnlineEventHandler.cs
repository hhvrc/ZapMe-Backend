using fbs.server;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ZapMe.BusinessLogic.CQRS.Events;
using ZapMe.Database;
using ZapMe.Websocket;

namespace ZapMe.BusinessLogic.CQRS.Handlers;

/*
public sealed class UserOnlineEventHandler : INotificationHandler<UserOnlineEvent>
{
    private readonly DatabaseContext _dbContext;

    public UserOnlineEventHandler(IServiceProvider serviceProvider)
    {
        _dbContext = serviceProvider.GetRequiredService<DatabaseContext>();
    }

    public async ValueTask Handle(UserOnlineEvent notification, CancellationToken cancellationToken)
    {
        var userStatus = await _dbContext.Users.Where(u => u.Id == notification.UserId).Include(u => u.RelationsIncoming).Select(u => new { u.Status, u.RelationsIncoming }).FirstOrDefaultAsync(cancellationToken);
        if (userStatus is null || userStatus.Status == Enums.UserStatus.Offline) return;

        var fbsStatus = userStatus.Status switch
        {
            Enums.UserStatus.Online => UserOnlineStatus.online,
            Enums.UserStatus.Offline => UserOnlineStatus.offline,
            Enums.UserStatus.DoNotDisturb => UserOnlineStatus.do_not_disturb,
            Enums.UserStatus.Inactive => UserOnlineStatus.inactive,
            _ => throw new NotImplementedException()
        };

        foreach (var relation in userStatus.RelationsIncoming.Where(r => r.FriendStatus == Enums.UserPartialRelationType.Accepted))
        {
            await WebSocketHub.RunActionOnUserAsync(relation.FromUserId, (client) => client.SendPayloadAsync(new ServerPayload(new UserOnlineStatusChanged
            {
                UserId = notification.UserId.ToString(),
                OnlineStatus = fbsStatus,
            }), cancellationToken));
        }
    }
}
*/
