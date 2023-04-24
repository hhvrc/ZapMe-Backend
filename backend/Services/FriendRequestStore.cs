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

    public async Task<FriendRequestEntity> CreateAsync(Guid senderId, Guid receiverId, CancellationToken cancellationToken)
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
}
