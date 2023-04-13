using Microsoft.EntityFrameworkCore;
using ZapMe.Constants;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class PasswordResetRequestStore : IPasswordResetRequestStore
{
    private readonly ZapMeContext _dbContext;
    private readonly ILogger<PasswordResetRequestStore> _logger;

    public PasswordResetRequestStore(ZapMeContext dbContext, ILogger<PasswordResetRequestStore> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<PasswordResetRequestEntity> UpsertAsync(Guid userId, string tokenHash, CancellationToken cancellationToken)
    {
        if (tokenHash.Length != HashConstants.Sha256LengthHex) throw new ArgumentException($"Tokenhash should be {HashConstants.Sha256LengthHex} characters", nameof(tokenHash));

        PasswordResetRequestEntity? request = await _dbContext.PasswordResetRequests.AsTracking().FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);

        if (request == null)
        {
            request = new PasswordResetRequestEntity
            {
                UserId = userId,
                User = null!,
                TokenHash = tokenHash
            };

            await _dbContext.PasswordResetRequests.AddAsync(request, cancellationToken);
        }
        else
        {
            request.TokenHash = tokenHash;
            request.CreatedAt = DateTime.UtcNow;

            _dbContext.PasswordResetRequests.Update(request);
        }

        int nAdded = await _dbContext.SaveChangesAsync(cancellationToken);

        if (nAdded <= 0)
        {
            _logger.LogWarning("Failed to create password reset request for user {UserId}", userId);
        }

        return request;
    }

    public Task<PasswordResetRequestEntity?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _dbContext.PasswordResetRequests
            .Include(static s => s.User)
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
    }

    public Task<PasswordResetRequestEntity?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return _dbContext.PasswordResetRequests
            .Include(static s => s.User)
            .FirstOrDefaultAsync(s => s.TokenHash == tokenHash, cancellationToken);
    }

    public async Task<bool> DeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.PasswordResetRequests.Where(s => s.UserId == userId).ExecuteDeleteAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return await _dbContext.PasswordResetRequests.Where(s => s.TokenHash == tokenHash).ExecuteDeleteAsync(cancellationToken) > 0;
    }

    public Task<int> DeleteExpiredAsync(TimeSpan maxAge, CancellationToken cancellationToken)
    {
        DateTime expiresAt = DateTime.UtcNow - maxAge;
        return _dbContext.PasswordResetRequests.Where(s => s.CreatedAt < expiresAt).ExecuteDeleteAsync(cancellationToken);
    }
}
