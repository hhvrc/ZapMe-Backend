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

    public Task<List<LockOutEntity>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task LockOutAsync(Guid userId, string? reason, string flags, DateTime? expiresAt, CancellationToken cancellationToken)
    {
        await LockOutStore.CreateAsync(userId, reason, flags, expiresAt, cancellationToken);
    }

    public async Task<bool> IsLockedOutAsync(Guid userId, CancellationToken cancellationToken)
    {
        var lockOuts = await LockOutStore.ListByUserAsync(userId, cancellationToken);
        return lockOuts?.Any(x => x.ExpiresAt > DateTime.UtcNow) ?? false;
    }
}
