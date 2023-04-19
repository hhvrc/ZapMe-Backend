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
    private readonly ILogger<LockOutStore> _logger;

    public LockOutStore(ZapMeContext dbContext, ILogger<LockOutStore> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<LockOutEntity> CreateAsync(Guid userId, string? reason, string flags, DateTime? expiresAt, CancellationToken cancellationToken)
    {
        LockOutEntity lockout = new LockOutEntity
        {
            User = null!, // TODO: wtf do i do now? ask on C# discord
            UserId = userId,
            Reason = reason,
            Flags = flags,
            ExpiresAt = expiresAt
        };

        await _dbContext.LockOuts.AddAsync(lockout, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return lockout;
    }

    public Task<LockOutEntity?> GetByIdAsync(Guid lockOutId, CancellationToken cancellationToken)
    {
        return _dbContext.LockOuts.FirstOrDefaultAsync(l => l.Id == lockOutId, cancellationToken);
    }

    public IAsyncEnumerable<LockOutEntity> ListByUserIdAsync(Guid userId)
    {
        return _dbContext.LockOuts.Where(l => l.UserId == userId).ToAsyncEnumerable();
    }

    private async Task<bool> UpdateAsync(Guid lockOutId, Expression<Func<SetPropertyCalls<LockOutEntity>, SetPropertyCalls<LockOutEntity>>> setPropertyCalls, CancellationToken cancellationToken)
    {
        return (await _dbContext.LockOuts.Where(l => l.Id == lockOutId).ExecuteUpdateAsync(setPropertyCalls, cancellationToken)) > 0;
    }

    public Task<bool> SetReasonAsync(Guid lockOutId, string reason, CancellationToken cancellationToken) =>
        UpdateAsync(lockOutId, s => s.SetProperty(static l => l.Reason, _ => reason), cancellationToken);
    public Task<bool> SetFlagsAsync(Guid lockOutId, string flags, CancellationToken cancellationToken) =>
        UpdateAsync(lockOutId, s => s.SetProperty(static l => l.Flags, _ => flags), cancellationToken);
    public Task<bool> SetExipresAtAsync(Guid lockOutId, DateTime expiresAt, CancellationToken cancellationToken) =>
        UpdateAsync(lockOutId, s => s.SetProperty(static l => l.ExpiresAt, _ => expiresAt), cancellationToken);

    public async Task<bool> DeleteAsync(Guid lockOutId, CancellationToken cancellationToken)
    {
        return await _dbContext.LockOuts.Where(l => l.Id == lockOutId).ExecuteDeleteAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAllAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.LockOuts.Where(l => l.UserId == userId).ExecuteDeleteAsync(cancellationToken) > 0;
    }
}
