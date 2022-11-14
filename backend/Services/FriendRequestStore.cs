using Microsoft.EntityFrameworkCore;
using Npgsql;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public class FriendRequestStore : IFriendRequestStore
{
    private readonly ZapMeContext _dbContext;
    private readonly IHybridCache _cache;
    private readonly ILogger<FriendRequestStore> _logger;

    public FriendRequestStore(ZapMeContext dbContext, IHybridCache cacheProviderService, ILogger<FriendRequestStore> logger)
    {
        _dbContext = dbContext;
        _cache = cacheProviderService;
        _logger = logger;
    }

    public async Task<FriendRequestEntity?> CreateAsync(Guid senderId, Guid receiverId, CancellationToken cancellationToken = default)
    {
        FriendRequestEntity? friendRequestEntity = null;

        try
        {
            friendRequestEntity = new FriendRequestEntity
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Sender = null!,
                Receiver = null!,
            };

            await _dbContext.FriendRequests.AddAsync(friendRequestEntity, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create friend request");
            friendRequestEntity = null;
        }

        return friendRequestEntity;
    }

    public async Task<FriendRequestEntity[]> ListByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        FriendRequestEntity[] friendRequestEntity;

        try
        {
            friendRequestEntity = await _dbContext.FriendRequests.Where(fr => fr.SenderId == userId || fr.ReceiverId == userId).ToArrayAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list friend requests by sender");
            friendRequestEntity = Array.Empty<FriendRequestEntity>();
        }

        return friendRequestEntity;
    }

    public async Task<FriendRequestEntity[]> ListBySenderAsync(Guid senderId, CancellationToken cancellationToken = default)
    {
        FriendRequestEntity[] friendRequestEntity;

        try
        {
            friendRequestEntity = await _dbContext.FriendRequests.Where(fr => fr.SenderId == senderId).ToArrayAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list friend requests by sender");
            friendRequestEntity = Array.Empty<FriendRequestEntity>();
        }
        
        return friendRequestEntity;
    }

    public async Task<FriendRequestEntity[]> ListByReceiverAsync(Guid receiverId, CancellationToken cancellationToken = default)
    {
        FriendRequestEntity[] friendRequestEntity;

        try
        {
            friendRequestEntity = await _dbContext.FriendRequests.Where(fr => fr.ReceiverId == receiverId).ToArrayAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list friend requests by receiver");
            friendRequestEntity = Array.Empty<FriendRequestEntity>();
        }

        return friendRequestEntity;
    }
}
