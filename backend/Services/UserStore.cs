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

public sealed class UserStore : IUserStore
{
    private readonly ZapMeContext _dbContext;
    private readonly ILogger<UserStore> _logger;

    public UserStore(ZapMeContext dbContext, ILogger<UserStore> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<AccountCreationResult> TryCreateAsync(string name, string email, string passwordHash, CancellationToken cancellationToken)
    {
        var user = new UserEntity
        {
            Name = name,
            Email = email,
            PasswordHash = passwordHash,
            AcceptedTosVersion = 0,
            ProfilePictureId = ImageEntity.DefaultImageId,
            OnlineStatus = UserOnlineStatus.Online,
            OnlineStatusText = String.Empty,
            UpdatedAt = DateTime.UtcNow
        };

        int retryCount = 0;
        while (retryCount < 3)
        {
            try
            {
                await _dbContext.Accounts.AddAsync(user, cancellationToken);
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
                    if (postgresException.ConstraintName == UserEntity.TableAccountEmailIndex)
                    {
                        return new AccountCreationResult(AccountCreationResult.ResultE.EmailAlreadyTaken, null!, exception.Message);
                    }
                    else if (postgresException.ConstraintName == UserEntity.TableAccountNameIndex)
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

    public Task<UserEntity?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _dbContext.Accounts.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public Task<UserEntity?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return _dbContext.Accounts.FirstOrDefaultAsync(u => u.Name == name, cancellationToken);
    }

    public Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _dbContext.Accounts.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public Task<UserEntity?> GetByNameOrEmail(string nameOrEmail, CancellationToken cancellationToken)
    {
        return _dbContext.Accounts.FirstOrDefaultAsync(u => u.Name == nameOrEmail || u.Email == nameOrEmail, cancellationToken);
    }

    private async Task<bool> UpdateAsync(Expression<Func<UserEntity, bool>> whereSelector, Expression<Func<SetPropertyCalls<UserEntity>, SetPropertyCalls<UserEntity>>> setPropertyCalls, CancellationToken cancellationToken)
    {
        return (await _dbContext.Accounts.Where(whereSelector).ExecuteUpdateAsync(setPropertyCalls, cancellationToken)) > 0;
    }
    private Task<bool> UpdateAsync(Guid userId, Expression<Func<SetPropertyCalls<UserEntity>, SetPropertyCalls<UserEntity>>> setPropertyCalls, CancellationToken cancellationToken) =>
        UpdateAsync(u => u.Id == userId, setPropertyCalls, cancellationToken);
    private Task<bool> UpdateAsync(string name, Expression<Func<SetPropertyCalls<UserEntity>, SetPropertyCalls<UserEntity>>> setPropertyCalls, CancellationToken cancellationToken) =>
        UpdateAsync(u => u.Name == name, setPropertyCalls, cancellationToken);

    public Task<bool> SetUserNameAsync(Guid userId, string name, CancellationToken cancellationToken) =>
        UpdateAsync(userId, s => s.SetProperty(static u => u.Name, _ => name).SetProperty(static u => u.UpdatedAt, _ => DateTime.UtcNow), cancellationToken);
    public Task<bool> SetEmailAsync(Guid userId, string email, CancellationToken cancellationToken) =>
        UpdateAsync(userId, s => s.SetProperty(static u => u.Email, _ => email).SetProperty(static u => u.UpdatedAt, _ => DateTime.UtcNow), cancellationToken);
    public Task<bool> SetEmailVerifiedAsync(Guid userId, bool emailVerified, CancellationToken cancellationToken) =>
        UpdateAsync(userId, s => s.SetProperty(static u => u.EmailVerified, _ => emailVerified).SetProperty(static u => u.UpdatedAt, _ => DateTime.UtcNow), cancellationToken);
    public Task<bool> SetPasswordHashAsync(Guid userId, string passwordHash, CancellationToken cancellationToken) =>
        UpdateAsync(userId, s => s.SetProperty(static u => u.PasswordHash, _ => passwordHash).SetProperty(static u => u.UpdatedAt, _ => DateTime.UtcNow), cancellationToken);
    public Task<bool> SetLastOnlineAsync(Guid userId, DateTime lastOnline, CancellationToken cancellationToken) =>
        UpdateAsync(userId, s => s.SetProperty(static u => u.LastOnline, _ => lastOnline), cancellationToken);

    public async Task<bool> DeleteAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Accounts.Where(u => u.Id == userId).ExecuteDeleteAsync(cancellationToken) > 0;
    }
}