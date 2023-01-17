using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class LockOutManager : ILockOutManager
{
    public ILockOutStore LockOutStore { get; }
    private readonly ILogger<LockOutManager> _logger;

    public LockOutManager(ILockOutStore lockOutStore, ILogger<LockOutManager> logger)
    {
        LockOutStore = lockOutStore;
        _logger = logger;
    }

    public Task<LockOutEntity> GetAsync(Guid lockOutId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<LockOutEntity> ListByUserIdAsync(Guid userId)
    {
        return LockOutStore.ListByUserAsync(userId);
    }

    public Task LockOutAsync(Guid userId, string? reason, string flags, DateTime? expiresAt, CancellationToken cancellationToken)
    {
        return LockOutStore.CreateAsync(userId, reason, flags, expiresAt, cancellationToken);
    }

    public ValueTask<bool> IsLockedOutAsync(Guid userId, CancellationToken cancellationToken)
    {
        return LockOutStore.ListByUserAsync(userId).AnyAsync(x => x.ExpiresAt > DateTime.UtcNow, cancellationToken);
    }
}
