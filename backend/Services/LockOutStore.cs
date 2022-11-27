using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class LockOutStore : ILockOutStore
{
    private readonly ZapMeContext _dbContext;
    private readonly IHybridCache _cache;
    private readonly ILogger<LockOutStore> _logger;

    public LockOutStore(ZapMeContext dbContext, IHybridCache cacheProviderService, ILogger<LockOutStore> logger)
    {
        _dbContext = dbContext;
        _cache = cacheProviderService;
        _logger = logger;
    }

    public async Task<LockOutEntity?> CreateAsync(Guid userId, string? reason, string flags, DateTime? expiresAt, CancellationToken cancellationToken)
    {
        var entity = new LockOutEntity
        {
            User = null!, // TODO: wtf do i do now? ask on C# discord
            UserId = userId,
            Reason = reason,
            Flags = flags,
            ExpiresAt = expiresAt
        };

        try
        {
            await _dbContext.LockOuts.AddAsync(entity, cancellationToken);
            int nAdded = await _dbContext.SaveChangesAsync(cancellationToken);

            if (nAdded > 0)
            {
                return entity;
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to create lockout for user {UserId}", userId);
        }

        return null;
    }
    
    public Task<LockOutEntity?> GetByIdAsync(Guid lockOutId, CancellationToken cancellationToken)
    {
        return _cache.GetOrAddAsync("lockOut:id:" + lockOutId, async (_, ct) =>
            new DTOs.HybridCacheEntry<LockOutEntity?>
            {
                Value = await _dbContext.LockOuts.FirstOrDefaultAsync(l => l.Id == lockOutId, ct),
                ExpiresAtUtc = DateTime.UtcNow + TimeSpan.FromMinutes(5)
            }
        , cancellationToken);
    }
    
    public Task<LockOutEntity[]> ListByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _cache.GetOrAddAsync("lockOut:user:" + userId, async (_, ct) =>
            new DTOs.HybridCacheEntry<LockOutEntity[]>
            {
                Value = await _dbContext.LockOuts.Where(l => l.UserId == userId).ToArrayAsync(ct),
                ExpiresAtUtc = DateTime.UtcNow + TimeSpan.FromMinutes(5)
            }
        , cancellationToken);
    }

    private async Task<bool> UpdateAsync(Guid lockOutId, Expression<Func<SetPropertyCalls<LockOutEntity>, SetPropertyCalls<LockOutEntity>>> setPropertyCalls, CancellationToken cancellationToken)
    {
        try
        {
            return (await _dbContext.LockOuts.Where(l => l.Id == lockOutId).ExecuteUpdateAsync(setPropertyCalls, cancellationToken)) > 0;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to update lockout {LockOutId}", lockOutId);
        }

        return false;
    }

    public Task<bool> SetReasonAsync(Guid lockOutId, string reason, CancellationToken cancellationToken) =>
        UpdateAsync(lockOutId, s => s.SetProperty(static l => l.Reason, _ => reason), cancellationToken);
    public Task<bool> SetFlagsAsync(Guid lockOutId, string flags, CancellationToken cancellationToken) =>
        UpdateAsync(lockOutId, s => s.SetProperty(static l => l.Flags, _ => flags), cancellationToken);
    public Task<bool> SetExipresAtAsync(Guid lockOutId, DateTime expiresAt, CancellationToken cancellationToken) =>
        UpdateAsync(lockOutId, s => s.SetProperty(static l => l.ExpiresAt, _ => expiresAt), cancellationToken);

    public async Task<bool> DeleteAsync(Guid lockOutId, CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.LockOuts.Where(l => l.Id == lockOutId).ExecuteDeleteAsync(cancellationToken) > 0;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to delete lockout {LockOutId}", lockOutId);
        }

        return false;
    }

    public async Task<bool> DeleteAllAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.LockOuts.Where(l => l.UserId == userId).ExecuteDeleteAsync(cancellationToken) > 0;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to delete all lockouts for user {UserId}", userId);
        }

        return false;
    }
}
