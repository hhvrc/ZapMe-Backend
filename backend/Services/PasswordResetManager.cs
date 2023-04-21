using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ZapMe.Constants;
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
    private readonly IMailTemplateStore _mailTemplateStore;
    private readonly IMailGunService _mailGunService;
    private readonly IPasswordResetRequestStore _passwordResetRequestStore;
    private readonly ILogger<PasswordResetManager> _logger;

    public PasswordResetManager(ZapMeContext dbContext, IUserManager userManager, IMailTemplateStore mailTemplateStore, IMailGunService mailGunService, IPasswordResetRequestStore passwordResetRequestStore, ILogger<PasswordResetManager> logger)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _mailTemplateStore = mailTemplateStore;
        _mailGunService = mailGunService;
        _passwordResetRequestStore = passwordResetRequestStore;
        _logger = logger;
    }

    // TODO: Make this method return a Any<> type, bool if success, string/IActionResult if not
    public async Task<bool> InitiatePasswordReset(UserEntity user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user.Email);

        string token = StringUtils.GenerateUrlSafeRandomString(16);
        string tokenHash = HashingUtils.Sha256_String(token);

        string? emailTemplate = await _mailTemplateStore.GetTemplateAsync(EmailTemplateNames.PasswordReset, cancellationToken);
        if (emailTemplate == null)
            return false;

        string formattedEmail = new QuickStringReplacer(emailTemplate)
            .Replace("{{UserName}}", user.Name)
            .Replace("{{ResetPasswordUrl}}", App.WebsiteUrl + "/reset-password?token=" + token)
            .Replace("{{CompanyName}}", App.AppCreator)
            .Replace("{{CompanyAddress}}", App.MadeInText)
            .Replace("{{PoweredBy}}", App.AppName)
            .Replace("{{PoweredByLink}}", App.WebsiteUrl)
            .ToString();

        bool success = false;

        IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _passwordResetRequestStore.UpsertAsync(user.Id, tokenHash, cancellationToken);

            // Send recovery secret to email
            success = await _mailGunService.SendEmailAsync("Hello", user.Name, user.Email, "Password recovery", formattedEmail, cancellationToken);
        }
        finally
        {
            if (success)
            {
                await transaction.CommitAsync(cancellationToken);
            }
            else
            {
                await transaction.RollbackAsync(cancellationToken);
            }
        }

        return success;
    }

    public async Task<bool> InitiatePasswordReset(Guid accountId, CancellationToken cancellationToken)
    {
        UserEntity? userEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == accountId && u.Email != null, cancellationToken);
        if (userEntity == null) return false;

        return await InitiatePasswordReset(userEntity, cancellationToken);
    }

    public async Task<bool> InitiatePasswordReset(string accountEmail, CancellationToken cancellationToken)
    {
        UserEntity? userEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == accountEmail && u.Email != null, cancellationToken);
        if (userEntity == null) return false;

        return await InitiatePasswordReset(userEntity, cancellationToken);
    }

    public async Task<bool> TryCompletePasswordReset(string token, string newPassword, CancellationToken cancellationToken)
    {
        string tokenHash = HashingUtils.Sha256_String(token);

        PasswordResetRequestEntity? passwordResetRequest = await _dbContext.PasswordResetRequests.FirstOrDefaultAsync(p => p.TokenHash == tokenHash, cancellationToken);
        if (passwordResetRequest == null) return false;

        // Check if token is valid
        if (passwordResetRequest.CreatedAt.AddMinutes(30) < DateTime.UtcNow) return false;

        string newPasswordHash = PasswordUtils.HashPassword(newPassword);

        bool success = false;
        IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Important to delete by token hash, and not account id as if the token has changed, someone else has issued a password reset
            bool deleted = await _dbContext.PasswordResetRequests.Where(p => p.TokenHash == tokenHash).ExecuteDeleteAsync(cancellationToken) > 0;
            if (!deleted) return false; // Another caller made it before we did

            // Finally set the new password
            success = await _dbContext.Users.Where(u => u.Id == passwordResetRequest.UserId).ExecuteUpdateAsync(spc => spc.SetProperty(u => u.PasswordHash, _ => newPasswordHash), cancellationToken) > 0;
        }
        finally
        {
            if (success)
            {
                await transaction.CommitAsync(cancellationToken);
            }
            else
            {
                await transaction.RollbackAsync(cancellationToken);
            }
        }

        return success;
    }

    public Task<int> RemoveExpiredRequests(CancellationToken cancellationToken)
    {
        DateTime minCreatedAt = DateTime.UtcNow - TimeSpan.FromMinutes(30);
        return _dbContext.PasswordResetRequests.Where(x => x.CreatedAt < minCreatedAt).ExecuteDeleteAsync(cancellationToken);
    }
}
