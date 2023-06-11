using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class UserStore : IUserStore
{
    private readonly DatabaseContext _dbContext;
    private readonly ILogger<UserStore> _logger;

    public UserStore(DatabaseContext dbContext, ILogger<UserStore> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> TryCreateAsync(UserEntity user, CancellationToken cancellationToken)
    {
        int retryCount = 0;
    retry:
        try
        {
            using IDbContextTransaction? transaction = await _dbContext.Database.BeginTransactionIfNotExistsAsync(cancellationToken);

            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            if (transaction != null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return true;
        }
        catch (PostgresException exception)
        {
            if (exception.IsTransient && retryCount++ < 3)
            {
                goto retry;
            }

            _logger.LogError("Ran out of retries while creating account!");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to create user account");
        }

        return false;
    }
}