using Microsoft.EntityFrameworkCore;
using Npgsql;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.DTOs;
using ZapMe.Enums;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Services;

public sealed class UserManager : IUserManager
{
    private readonly ZapMeContext _dbContext;
    private readonly ILogger<UserManager> _logger;

    public UserManager(ZapMeContext dbContext, ILogger<UserManager> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<AccountCreationResult> TryCreateAsync(string name, string? email, string password, CancellationToken cancellationToken)
    {
        string passwordHash = PasswordUtils.HashPassword(password);

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

    public async Task<PasswordCheckResult> CheckPasswordAsync(Guid userId, string password, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.Where(u => u.Id == userId).Select(u => new { u.PasswordHash }).FirstOrDefaultAsync(cancellationToken);
        if (user == null)
        {
            return PasswordCheckResult.UserNotFound;
        }

        if (!PasswordUtils.CheckPassword(password, user.PasswordHash))
        {
            return PasswordCheckResult.PasswordInvalid;
        }

        return PasswordCheckResult.Success;
    }
}