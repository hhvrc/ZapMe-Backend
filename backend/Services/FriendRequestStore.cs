using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class FriendRequestStore : IFriendRequestStore
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
        FriendRequestEntity friendRequest = new FriendRequestEntity
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Sender = null!,
            Receiver = null!,
        };

        await _dbContext.FriendRequests.AddAsync(friendRequest, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return friendRequest;
    }

    public IAsyncEnumerable<FriendRequestEntity> ListByUserAsync(Guid userId)
    {
        return _dbContext.FriendRequests.Where(fr => fr.SenderId == userId || fr.ReceiverId == userId).ToAsyncEnumerable();
    }

    public IAsyncEnumerable<FriendRequestEntity> ListBySenderAsync(Guid senderId)
    {
        return _dbContext.FriendRequests.Where(fr => fr.SenderId == senderId).ToAsyncEnumerable();
    }

    public IAsyncEnumerable<FriendRequestEntity> ListByReceiverAsync(Guid receiverId)
    {
        return _dbContext.FriendRequests.Where(fr => fr.ReceiverId == receiverId).ToAsyncEnumerable();
    }
}
