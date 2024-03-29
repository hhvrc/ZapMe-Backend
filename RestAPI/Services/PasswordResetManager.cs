﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ZapMe.Constants;
using ZapMe.Database;
using ZapMe.Database.Extensions;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

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
        bool success = await _mailGunService.SendEmailAsync(App.AppName, "support", user.Name, user.Email, "Password recovery", "password-reset", mailgunValues, cancellationToken);
        if (!success)
        {
            _logger.LogError("Failed to send password reset email to {Email}", user.Email);
            return HttpErrors.InternalServerError;
        }

        // Commit transaction
        if (transaction is not null)
        {
            await transaction.CommitAsync(cancellationToken);
        }

        return null;
    }

    public async Task<bool> TryCompletePasswordReset(string token, string newPassword, CancellationToken cancellationToken)
    {
        string tokenHash = HashingUtils.Sha256_Hex(token);

        UserPasswordResetRequestEntity? passwordResetRequest = await _dbContext
            .UserPasswordResetRequests
            .FirstOrDefaultAsync(p => p.TokenHash == tokenHash, cancellationToken);
        if (passwordResetRequest is null) return false;

        // Check if token is valid
        if (passwordResetRequest.CreatedAt.AddMinutes(30) < DateTime.UtcNow) return false;

        string newPasswordHash = PasswordUtils.HashPassword(newPassword);

        // Start transaction
        using IDbContextTransaction? transaction = await _dbContext.Database.BeginTransactionIfNotExistsAsync(cancellationToken);

        // Important to delete by token hash, and not account id as if the token has changed, someone else has issued a password reset
        int nDeleted = await _dbContext
            .UserPasswordResetRequests
            .Where(p => p.TokenHash == tokenHash)
            .ExecuteDeleteAsync(cancellationToken);
        if (nDeleted <= 0) return false; // Another caller made it before we did

        // Finally set the new password
        int nUpdated = await _dbContext
            .Users
            .Where(u => u.Id == passwordResetRequest.UserId)
            .ExecuteUpdateAsync(spc => spc
                .SetProperty(u => u.PasswordHash, _ => newPasswordHash), cancellationToken);
        if (nDeleted <= 0) return false; // Uhh, race condition?

        // Commit transaction
        if (transaction is not null)
        {
            await transaction.CommitAsync(cancellationToken);
        }

        return true;
    }

    public Task<int> RemoveExpiredRequests(CancellationToken cancellationToken)
    {
        DateTime expiryDate = DateTime.UtcNow.AddHours(-24);
        return _dbContext
            .UserPasswordResetRequests
            .Where(x => x.CreatedAt < expiryDate)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
