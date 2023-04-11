using ZapMe.Constants;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data.Models;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Services;

public sealed class PasswordResetRequestManager : IPasswordResetRequestManager
{
    private readonly IAccountManager _accountManager;
    private readonly IMailTemplateStore _mailTemplateStore;
    private readonly IMailGunService _mailGunService;
    private readonly IPasswordResetRequestStore _passwordResetRequestStore;
    private readonly ILogger<PasswordResetRequestManager> _logger;

    public PasswordResetRequestManager(IAccountManager accountManager, IMailTemplateStore mailTemplateStore, IMailGunService mailGunService, IPasswordResetRequestStore passwordResetRequestStore, ILogger<PasswordResetRequestManager> logger)
    {
        _accountManager = accountManager;
        _mailTemplateStore = mailTemplateStore;
        _mailGunService = mailGunService;
        _passwordResetRequestStore = passwordResetRequestStore;
        _logger = logger;
    }

    public async Task<bool> InitiatePasswordReset(AccountEntity account, CancellationToken cancellationToken)
    {
        string token = StringUtils.GenerateRandomString(16);
        string tokenHash = HashingUtils.Sha256_String(token);

        await _passwordResetRequestStore.UpsertAsync(account.Id, tokenHash, cancellationToken);

        string? emailTemplate = await _mailTemplateStore.GetTemplateAsync(EmailTemplateNames.PasswordReset, cancellationToken);
        if (emailTemplate == null) return false;

        string formattedEmail = new QuickStringReplacer(emailTemplate)
            .Replace("{{UserName}}", account.Name)
            .Replace("{{ResetPasswordUrl}}", App.WebsiteUrl + "/reset-password?token=" + token)
            .Replace("{{CompanyName}}", App.AppCreator)
            .Replace("{{CompanyAddress}}", App.MadeInText)
            .Replace("{{PoweredBy}}", App.AppName)
            .Replace("{{PoweredByLink}}", App.WebsiteUrl)
            .ToString();

        // Send recovery secret to email
        return await _mailGunService.SendEmailAsync("Hello", account.Name, account.Email, "Password recovery", formattedEmail, cancellationToken);
    }

    public async Task<bool> InitiatePasswordReset(Guid accountId, CancellationToken cancellationToken)
    {
        AccountEntity? accountEntity = await _accountManager.GetByIdAsync(accountId, cancellationToken);
        if (accountEntity == null) return false;

        return await InitiatePasswordReset(accountEntity, cancellationToken);
    }

    public async Task<bool> InitiatePasswordReset(string accountEmail, CancellationToken cancellationToken)
    {
        AccountEntity? accountEntity = await _accountManager.GetByEmailAsync(accountEmail, cancellationToken);
        if (accountEntity == null) return false;

        return await InitiatePasswordReset(accountEntity, cancellationToken);
    }

    public async Task<bool> CompletePasswordReset(string token, string newPassword, CancellationToken cancellationToken)
    {
        string tokenHash = HashingUtils.Sha256_String(token);

        PasswordResetRequestEntity? passwordResetRequest = await _passwordResetRequestStore.GetByTokenHashAsync(tokenHash, cancellationToken);
        if (passwordResetRequest == null) return false;

        // Important to delete by token hash, and not account id as if the token has changed, someone else has issued a password reset
        bool deleted = await _passwordResetRequestStore.DeleteByTokenHashAsync(tokenHash, cancellationToken);
        if (!deleted) return false; // Another caller made it before we did

        // Check if token is valid
        if (passwordResetRequest.CreatedAt.AddMinutes(30) < DateTime.UtcNow) return false;

        // Finally set the new password
        return await _accountManager.SetPasswordAsync(passwordResetRequest.AccountId, newPassword, cancellationToken);
    }

    public Task<int> RemoveExpiredRequests(CancellationToken cancellationToken)
    {
        return _passwordResetRequestStore.DeleteExpiredAsync(TimeSpan.FromMinutes(30), cancellationToken);
    }
}
