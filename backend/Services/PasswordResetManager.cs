using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OneOf;
using ZapMe.Constants;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Services;

public sealed class PasswordResetManager : IPasswordResetManager
{
    private readonly ZapMeContext _dbContext;
    private readonly IUserManager _userManager;
    private readonly IEmailTemplateStore _mailTemplateStore;
    private readonly IMailGunService _mailGunService;
    private readonly IPasswordResetRequestStore _passwordResetRequestStore;
    private readonly ILogger<PasswordResetManager> _logger;

    public PasswordResetManager(ZapMeContext dbContext, IUserManager userManager, IEmailTemplateStore emailTemplateStore, IMailGunService emailGunService, IPasswordResetRequestStore passwordResetRequestStore, ILogger<PasswordResetManager> logger)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _mailTemplateStore = emailTemplateStore;
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

        OneOf<string, ErrorDetails> getTemplateResponse = await _mailTemplateStore.GetTemplateAsync(EmailTemplateNames.PasswordReset, cancellationToken);
        if (getTemplateResponse.TryPickT1(out ErrorDetails errorDetails, out string emailTemplate))
        {
            return errorDetails;
        }

        string formattedEmail = new QuickStringReplacer(emailTemplate)
            .Replace("{{UserName}}", user.Name)
            .Replace("{{ResetPasswordUrl}}", App.WebsiteUrl + "/reset-password?token=" + token)
            .Replace("{{CompanyName}}", App.AppCreator)
            .Replace("{{CompanyAddress}}", App.MadeInText)
            .Replace("{{PoweredBy}}", App.AppName)
            .Replace("{{PoweredByLink}}", App.WebsiteUrl)
            .ToString();

        // Start transaction
        using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        // Insert token into database
        await _passwordResetRequestStore.UpsertAsync(user.Id, tokenHash, cancellationToken);

        // Send recovery secret to email
        bool success = await _mailGunService.SendEmailAsync("Hello", user.Name, user.Email, "Password recovery", formattedEmail, cancellationToken);

        // Commit transaction
        await transaction.CommitAsync(cancellationToken);

        return null;
    }

    public async Task<ErrorDetails?> InitiatePasswordReset(Guid accountId, CancellationToken cancellationToken)
    {
        UserEntity? userEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == accountId && u.Email != null, cancellationToken);
        if (userEntity == null)
        {
            return CreateHttpError.Generic(StatusCodes.Status404NotFound, "Account not found", "The account was not found");
        }

        return await InitiatePasswordReset(userEntity, cancellationToken);
    }

    public async Task<ErrorDetails?> InitiatePasswordReset(string accountEmail, CancellationToken cancellationToken)
    {
        UserEntity? userEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == accountEmail && u.Email != null, cancellationToken);
        if (userEntity == null)
        {
            return CreateHttpError.Generic(StatusCodes.Status404NotFound, "Account not found", "The account associated with that email was not found");
        }

        return await InitiatePasswordReset(userEntity, cancellationToken);
    }

    public async Task<bool> TryCompletePasswordReset(string token, string newPassword, CancellationToken cancellationToken)
    {
        string tokenHash = HashingUtils.Sha256_Hex(token);

        PasswordResetRequestEntity? passwordResetRequest = await _dbContext.PasswordResetRequests.FirstOrDefaultAsync(p => p.TokenHash == tokenHash, cancellationToken);
        if (passwordResetRequest == null) return false;

        // Check if token is valid
        if (passwordResetRequest.CreatedAt.AddMinutes(30) < DateTime.UtcNow) return false;

        string newPasswordHash = PasswordUtils.HashPassword(newPassword);

        // Start transaction
        using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        // Important to delete by token hash, and not account id as if the token has changed, someone else has issued a password reset
        bool deleted = await _dbContext.PasswordResetRequests
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
        await transaction.CommitAsync(cancellationToken);

        return success;
    }

    public Task<int> RemoveExpiredRequests(CancellationToken cancellationToken)
    {
        DateTime minCreatedAt = DateTime.UtcNow - TimeSpan.FromMinutes(30);
        return _dbContext.PasswordResetRequests
            .Where(x => x.CreatedAt < minCreatedAt)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
