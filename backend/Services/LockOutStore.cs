﻿using ZapMe.Data;
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
            UserId = userId,
            Reason = reason,
            Flags = flags,
            ExpiresAt = expiresAt
        };

        await _dbContext.LockOuts.AddAsync(lockout, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return lockout;
    }
}
