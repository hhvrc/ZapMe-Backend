using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using ZapMe.Constants;
using ZapMe.DTOs;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;
using ZapMe.Database.Models;
using ZapMe.Database;

namespace ZapMe.Services;

public sealed class PasswordResetManager : IPasswordResetManager
{
    private readonly DatabaseContext _dbContext;
    private readonly IMailGunService _mailGunService;
    private readonly IPasswordResetRequestStore _passwordResetRequestStore;
    private readonly ILogger<PasswordResetManager> _logger;

    public PasswordResetManager(DatabaseContext dbContext, IMailGunService emailGunService, IPasswordResetRequestStore passwordResetRequestStore, ILogger<PasswordResetManager> logger)
    {
        _dbContext = dbContext;
        _mailGunService = emailGunService;
        _passwordResetRequestStore = passwordResetRequestStore;
        _logger = logger;
    }

    // TODO: Make this method return a Any<> type, bool if success, string/IActionResult if not
    public async Task<ErrorDetails?> InitiatePasswordReset(UserEntity user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user.Email);

        string token = StringUtils.GenerateUrlSafeRandomString(16);
        string tokenHash = HashingUtils.Sha256_Hex(token);

        Dictionary<string, string> mailgunValues = new Dictionary<string, string>
        {
            { "UserName", user.Name },
            { "ResetPasswordUrl", App.WebsiteUrl + "/reset-password?token=" + token },
            { "ContactLink", App.ContactUrl },
            { "PrivacyPolicyLink", App.PrivacyPolicyUrl },
            { "TermsOfServiceLink", App.TermsOfServiceUrl },
            { "CompanyName", App.AppCreator },
            { "CompanyAddress", App.MadeInText },
            { "PoweredBy", App.AppName },
            { "PoweredByLink", App.WebsiteUrl }
        };

        // Start transaction
        using IDbContextTransaction? transaction = await _dbContext.Database.BeginTransactionIfNotExistsAsync(cancellationToken);

        // Insert token into database
        await _passwordResetRequestStore.UpsertAsync(user.Id, tokenHash, cancellationToken);

        // Send recovery secret to email
        bool success = await _mailGunService.SendEmailAsync("Hello", user.Name, user.Email, "Password recovery", "password-reset", mailgunValues, cancellationToken);
        if (!success)
        {
            _logger.LogError("Failed to send password reset email to {Email}", user.Email);
            return HttpErrors.InternalServerError;
        }

        // Commit transaction
        if (transaction != null)
        {
            await transaction.CommitAsync(cancellationToken);
        }

        return null;
    }

    public async Task<ErrorDetails?> InitiatePasswordReset(Guid accountId, CancellationToken cancellationToken)
    {
        UserEntity? userEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == accountId && u.Email != null, cancellationToken);
        if (userEntity == null)
        {
            return HttpErrors.Generic(StatusCodes.Status404NotFound, "Account not found", "The account was not found");
        }

        return await InitiatePasswordReset(userEntity, cancellationToken);
    }

    public async Task<ErrorDetails?> InitiatePasswordReset(string accountEmail, CancellationToken cancellationToken)
    {
        UserEntity? userEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == accountEmail && u.Email != null, cancellationToken);
        if (userEntity == null)
        {
            return HttpErrors.Generic(StatusCodes.Status404NotFound, "Account not found", "The account associated with that email was not found");
        }

        return await InitiatePasswordReset(userEntity, cancellationToken);
    }

    public async Task<bool> TryCompletePasswordReset(string token, string newPassword, CancellationToken cancellationToken)
    {
        string tokenHash = HashingUtils.Sha256_Hex(token);

        UserPasswordResetRequestEntity? passwordResetRequest = await _dbContext.UserPasswordResetRequests.FirstOrDefaultAsync(p => p.TokenHash == tokenHash, cancellationToken);
        if (passwordResetRequest == null) return false;

        // Check if token is valid
        if (passwordResetRequest.CreatedAt.AddMinutes(30) < DateTime.UtcNow) return false;

        string newPasswordHash = PasswordUtils.HashPassword(newPassword);

        // Start transaction
        using IDbContextTransaction? transaction = await _dbContext.Database.BeginTransactionIfNotExistsAsync(cancellationToken);

        // Important to delete by token hash, and not account id as if the token has changed, someone else has issued a password reset
        bool deleted = await _dbContext.UserPasswordResetRequests
            .Where(p => p.TokenHash == tokenHash)
            .ExecuteDeleteAsync(cancellationToken) > 0;
        if (!deleted) return false; // Another caller made it before we did

        // Finally set the new password
        bool success = await _dbContext.Users
            .Where(u => u.Id == passwordResetRequest.UserId)
            .ExecuteUpdateAsync(spc => spc
            .SetProperty(u => u.PasswordHash, _ => newPasswordHash)
            , cancellationToken) > 0;
        if (!success) return false; // Uhh, race condition?

        // Commit transaction
        if (transaction != null)
        {
            await transaction.CommitAsync(cancellationToken);
        }

        return success;
    }

    public Task<int> RemoveExpiredRequests(CancellationToken cancellationToken)
    {
        return _dbContext.UserPasswordResetRequests
            .Where(x => x.CreatedAt < DateTime.UtcNow.AddHours(-24))
            .ExecuteDeleteAsync(cancellationToken);
    }
}
