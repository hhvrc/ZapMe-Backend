using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class FriendRequestStore : IFriendRequestStore
{
    private readonly DatabaseContext _dbContext;
    private readonly ILogger<FriendRequestStore> _logger;

    public FriendRequestStore(DatabaseContext dbContext, ILogger<FriendRequestStore> logger)
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
