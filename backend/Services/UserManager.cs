using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using OneOf;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Enums;
using ZapMe.Helpers;
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

    public async Task<OneOf<UserEntity, ErrorDetails>> TryCreateAsync(string name, string email, string password, bool emailVerified, CancellationToken cancellationToken)
    {
        if (_dbContext.Users.Any(u => u.Name == name || u.Email == email))
        {
            return CreateHttpError.UserNameOrEmailTaken();
        }

        string passwordHash = PasswordUtils.HashPassword(password);
        UserEntity user = new UserEntity
        {
            Name = name,
            Email = email,
            EmailVerified = emailVerified,
            PasswordHash = passwordHash,
            AcceptedPrivacyPolicyVersion = 0,
            AcceptedTermsOfServiceVersion = 0,
            ProfilePictureId = ImageEntity.DefaultImageId,
            OnlineStatus = UserStatus.Online,
            OnlineStatusText = String.Empty
        };

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

            return user;
        }
        catch (PostgresException exception)
        {
            if (exception.IsTransient && retryCount++ < 3)
            {
                goto retry;
            }

            _logger.LogError(exception, "Failed to create user account");

        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to create user account");
        }

        if (retryCount >= 3)
        {
            _logger.LogError("Ran out of retries while creating account!");
        }

        return CreateHttpError.InternalServerError();
    }
}