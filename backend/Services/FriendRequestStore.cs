using Microsoft.EntityFrameworkCore;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public class FriendRequestStore : IFriendRequestStore
{
    private readonly ZapMeContext _dbContext;
    private readonly ILogger<FriendRequestStore> _logger;

    public FriendRequestStore(ZapMeContext dbContext, ILogger<FriendRequestStore> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<FriendRequestEntity> CreateAsync(Guid senderId, Guid receiverId, CancellationToken cancellationToken = default)
    {
        FriendRequestEntity friendRequestEntity = new FriendRequestEntity
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Sender = null!,
            Receiver = null!,
        };

        await _dbContext.FriendRequests.AddAsync(friendRequestEntity, cancellationToken);

        return friendRequestEntity;
    }

    public Task<FriendRequestEntity[]> ListByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.FriendRequests.Where(fr => fr.SenderId == userId || fr.ReceiverId == userId).ToArrayAsync(cancellationToken);
    }

    public Task<FriendRequestEntity[]> ListBySenderAsync(Guid senderId, CancellationToken cancellationToken = default)
    {
        return _dbContext.FriendRequests.Where(fr => fr.SenderId == senderId).ToArrayAsync(cancellationToken);
    }

    public Task<FriendRequestEntity[]> ListByReceiverAsync(Guid receiverId, CancellationToken cancellationToken = default)
    {
        return _dbContext.FriendRequests.Where(fr => fr.ReceiverId == receiverId).ToArrayAsync(cancellationToken);
    }
}
