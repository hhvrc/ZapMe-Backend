using Microsoft.EntityFrameworkCore;
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

    public async Task<PasswordResetRequestEntity> UpsertAsync(Guid accountId, string tokenHash, CancellationToken cancellationToken)
    {
        if (tokenHash.Length != 32) throw new ArgumentException("Tokenhash should be 32 characters ( hex of sha256 digest )", nameof(tokenHash));

        PasswordResetRequestEntity? request = await _dbContext.PasswordResetRequests.AsTracking().FirstOrDefaultAsync(s => s.AccountId == accountId, cancellationToken);

        if (request == null)
        {
            request = new PasswordResetRequestEntity
            {
                AccountId = accountId,
                Account = null!,
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
            _logger.LogWarning("Failed to create password reset request for user {UserId}", accountId);
        }

        return request;
    }

    public Task<PasswordResetRequestEntity?> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken)
    {
        return _dbContext.PasswordResetRequests
            .Include(static s => s.Account)
            .FirstOrDefaultAsync(s => s.AccountId == accountId, cancellationToken);
    }

    public Task<PasswordResetRequestEntity?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return _dbContext.PasswordResetRequests
            .Include(static s => s.Account)
            .FirstOrDefaultAsync(s => s.TokenHash == tokenHash, cancellationToken);
    }

    public async Task<bool> DeleteByAccountIdAsync(Guid accountId, CancellationToken cancellationToken)
    {
        return await _dbContext.PasswordResetRequests.Where(s => s.AccountId == accountId).ExecuteDeleteAsync(cancellationToken) > 0;
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
