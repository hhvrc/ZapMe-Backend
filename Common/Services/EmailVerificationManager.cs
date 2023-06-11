﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using ZapMe.Constants;
using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Services;

public sealed class EmailVerificationManager : IEmailVerificationManager
{
    private readonly DatabaseContext _dbContext;
    private readonly IMailGunService _mailGunService;

    public EmailVerificationManager(DatabaseContext dbContext, IMailGunService mailGunService)
    {
        _dbContext = dbContext;
        _mailGunService = mailGunService;
    }

    public async Task<ErrorDetails?> InitiateEmailVerificationAsync(UserEntity user, string newEmail, CancellationToken cancellationToken)
    {
        // Create email verification entry
        string emailVerificationToken = StringUtils.GenerateUrlSafeRandomString(16);
        UserEmailVerificationRequestEntity? emailVerificationRequest = new UserEmailVerificationRequestEntity
        {
            UserId = user.Id,
            NewEmail = newEmail,
            TokenHash = HashingUtils.Sha256_Hex(emailVerificationToken)
        };

        Dictionary<string, string> mailgunValues = new Dictionary<string, string>
        {
            { "UserName", user.Name },
            { "ConfirmEmailLink", App.WebsiteUrl + "/verify-email?token=" + emailVerificationToken },
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

        // Save email verification request
        await _dbContext.UserEmailVerificationRequests.AddAsync(emailVerificationRequest, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Send email
        bool success = await _mailGunService.SendEmailAsync("System", user.Name, newEmail, "Account Created", "account-created", mailgunValues, cancellationToken);
        if (!success)
        {
            return HttpErrors.InternalServerError;
        }

        // Commit transaction
        if (transaction != null)
        {
            await transaction.CommitAsync(cancellationToken);
        }

        return null;
    }

    public async Task<ErrorDetails?> CompleteEmailVerificationAsync(string token, CancellationToken cancellationToken)
    {
        string tokenHash = HashingUtils.Sha256_Hex(token);

        // Fetch email verification request
        UserEmailVerificationRequestEntity? verificationRequest = await _dbContext.UserEmailVerificationRequests.SingleOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
        if (verificationRequest == null)
        {
            return HttpErrors.Generic(StatusCodes.Status404NotFound, "Invalid token", "Token invalid, expired, or already used");
        }

        // Fetch user
        int nUpdated = await _dbContext.Users
                                .Where(x => x.Id == verificationRequest.UserId && !x.EmailVerified && x.Email == verificationRequest.NewEmail)
                                .ExecuteUpdateAsync(spc => spc.SetProperty(x => x.EmailVerified, _ => true), cancellationToken);

        // Delete email verification request
        await _dbContext.UserEmailVerificationRequests.Where(x => x.TokenHash == tokenHash).ExecuteDeleteAsync(cancellationToken);

        if (nUpdated == 0)
        {
            return HttpErrors.Generic(StatusCodes.Status404NotFound, "Invalid token", "Token invalid, expired, or already used");
        }

        return null;
    }

    public Task<int> RemoveExpiredRequestsAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.UserEmailVerificationRequests
            .Where(x => x.CreatedAt < DateTime.UtcNow.AddHours(-24))
            .ExecuteDeleteAsync(cancellationToken);
    }
}