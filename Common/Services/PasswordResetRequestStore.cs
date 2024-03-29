﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZapMe.Constants;
using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class PasswordResetRequestStore : IPasswordResetRequestStore
{
    private readonly DatabaseContext _dbContext;
    private readonly ILogger<PasswordResetRequestStore> _logger;

    public PasswordResetRequestStore(DatabaseContext dbContext, ILogger<PasswordResetRequestStore> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<UserPasswordResetRequestEntity> UpsertAsync(Guid userId, string tokenHash, CancellationToken cancellationToken)
    {
        if (tokenHash.Length != HashConstants.Sha256LengthHex) throw new ArgumentException($"Tokenhash should be {HashConstants.Sha256LengthHex} characters", nameof(tokenHash));

        UserPasswordResetRequestEntity? request = await _dbContext.UserPasswordResetRequests.FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);

        if (request is null)
        {
            request = new UserPasswordResetRequestEntity
            {
                UserId = userId,
                TokenHash = tokenHash
            };

            _dbContext.UserPasswordResetRequests.Add(request);
        }
        else
        {
            request.TokenHash = tokenHash;
            request.CreatedAt = DateTime.UtcNow;

            _dbContext.UserPasswordResetRequests.Update(request);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return request;
    }
}
