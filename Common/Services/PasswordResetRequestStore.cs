using Microsoft.EntityFrameworkCore;
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

        UserPasswordResetRequestEntity? request = await _dbContext.UserPasswordResetRequests.AsTracking().FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);

        if (request == null)
        {
            request = new UserPasswordResetRequestEntity
            {
                UserId = userId,
                User = null!,
                TokenHash = tokenHash
            };

            await _dbContext.UserPasswordResetRequests.AddAsync(request, cancellationToken);
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
