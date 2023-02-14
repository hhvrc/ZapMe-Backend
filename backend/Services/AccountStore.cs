using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Npgsql;
using System.Linq.Expressions;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.DTOs;
using ZapMe.Enums;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class AccountStore : IAccountStore
{
    private readonly ZapMeContext _dbContext;
    private readonly ILogger<AccountStore> _logger;

    public AccountStore(ZapMeContext dbContext, ILogger<AccountStore> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<AccountCreationResult> TryCreateAsync(string username, string email, string passwordHash, CancellationToken cancellationToken)
    {
        var user = new AccountEntity
        {
            Name = username,
            Email = email,
            PasswordHash = passwordHash,
            OnlineStatus = UserOnlineStatus.Online,
            OnlineStatusText = String.Empty,
            UpdatedAt = DateTime.UtcNow
        };

        int retryCount = 0;
        while (retryCount < 3)
        {
            try
            {
                await _dbContext.Users.AddAsync(user, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return new AccountCreationResult(AccountCreationResult.ResultE.Success, user);
            }
            catch (DbUpdateException exception)
            {
                if (exception.InnerException is not PostgresException postgresException)
                {
                    return new AccountCreationResult(AccountCreationResult.ResultE.UnknownError, null!, exception.Message);
                }

                if (postgresException.IsTransient)
                {
                    retryCount++;
                    continue;
                }

                if (postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                    if (postgresException.ConstraintName == AccountEntity.TableAccountEmailIndex)
                    {
                        return new AccountCreationResult(AccountCreationResult.ResultE.EmailAlreadyTaken, null!, exception.Message);
                    }
                    else if (postgresException.ConstraintName == AccountEntity.TableAccountNameIndex)
                    {
                        return new AccountCreationResult(AccountCreationResult.ResultE.NameAlreadyTaken, null!, exception.Message);
                    }
                }

                var errorCode = postgresException.SqlState switch
                {
                    "23514" => AccountCreationResult.ResultE.NameOrEmailInvalid,
                    _ => AccountCreationResult.ResultE.UnknownError
                };

                return new AccountCreationResult(errorCode, null!, postgresException.MessageText);
            }
        }

        return new AccountCreationResult(AccountCreationResult.ResultE.UnknownError, null!, "Unknown error, retry count exceeded");
    }

    public Task<AccountEntity?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public Task<AccountEntity?> GetByNameAsync(string userName, CancellationToken cancellationToken)
    {
        return _dbContext.Users.FirstOrDefaultAsync(u => u.Name == userName, cancellationToken);
    }

    public Task<AccountEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public Task<AccountEntity?> GetByUsernameOrEmail(string userNameOrEmail, CancellationToken cancellationToken)
    {
        return _dbContext.Users.FirstOrDefaultAsync(u => u.Name == userNameOrEmail || u.Email == userNameOrEmail, cancellationToken);
    }

    public Task<AccountEntity?> GetByPasswordResetTokenAsync(string passwordResetToken, CancellationToken cancellationToken)
    {
        return _dbContext.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == passwordResetToken, cancellationToken);
    }

    private async Task<bool> UpdateAsync(Expression<Func<AccountEntity, bool>> whereSelector, Expression<Func<SetPropertyCalls<AccountEntity>, SetPropertyCalls<AccountEntity>>> setPropertyCalls, CancellationToken cancellationToken)
    {
        return (await _dbContext.Users.Where(whereSelector).ExecuteUpdateAsync(setPropertyCalls, cancellationToken)) > 0;
    }
    private Task<bool> UpdateAsync(Guid userId, Expression<Func<SetPropertyCalls<AccountEntity>, SetPropertyCalls<AccountEntity>>> setPropertyCalls, CancellationToken cancellationToken) =>
        UpdateAsync(u => u.Id == userId, setPropertyCalls, cancellationToken);
    private Task<bool> UpdateAsync(string userName, Expression<Func<SetPropertyCalls<AccountEntity>, SetPropertyCalls<AccountEntity>>> setPropertyCalls, CancellationToken cancellationToken) =>
        UpdateAsync(u => u.Name == userName, setPropertyCalls, cancellationToken);

    public Task<bool> SetUserNameAsync(Guid userId, string userName, CancellationToken cancellationToken) =>
        UpdateAsync(userId, s => s.SetProperty(static u => u.Name, _ => userName).SetProperty(static u => u.UpdatedAt, _ => DateTime.UtcNow), cancellationToken);
    public Task<bool> SetEmailAsync(Guid userId, string email, CancellationToken cancellationToken) =>
        UpdateAsync(userId, s => s.SetProperty(static u => u.Email, _ => email).SetProperty(static u => u.UpdatedAt, _ => DateTime.UtcNow), cancellationToken);
    public Task<bool> SetEmailVerifiedAsync(Guid userId, bool emailVerified, CancellationToken cancellationToken) =>
        UpdateAsync(userId, s => s.SetProperty(static u => u.EmailVerified, _ => emailVerified).SetProperty(static u => u.UpdatedAt, _ => DateTime.UtcNow), cancellationToken);
    public Task<bool> SetPasswordHashAsync(Guid userId, string passwordHash, CancellationToken cancellationToken) =>
        UpdateAsync(userId, s => s.SetProperty(static u => u.PasswordHash, _ => passwordHash).SetProperty(static u => u.UpdatedAt, _ => DateTime.UtcNow), cancellationToken);
    public Task<bool> SetLastOnlineAsync(Guid userId, DateTime lastOnline, CancellationToken cancellationToken) =>
        UpdateAsync(userId, s => s.SetProperty(static u => u.LastOnline, _ => lastOnline), cancellationToken);

    public Task<bool> SetPasswordResetTokenAsync(Guid userId, string? token, CancellationToken cancellationToken)
    {
        DateTime? createdAt = String.IsNullOrEmpty(token) ? null : DateTime.UtcNow;

        return UpdateAsync(userId, s => s.
                        SetProperty(static u => u.PasswordResetToken, _ => token).
                        SetProperty(static u => u.PasswordResetRequestedAt, _ => createdAt),
                        cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.Where(u => u.Id == userId).ExecuteDeleteAsync(cancellationToken) > 0;
    }
}