using ZapMe.Constants;
using ZapMe.Data.Models;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Services;

public sealed class PasswordResetManager : IPasswordResetManager
{
    private readonly IUserManager _userManager;
    private readonly IMailTemplateStore _mailTemplateStore;
    private readonly IMailGunService _mailGunService;
    private readonly IPasswordResetRequestStore _passwordResetRequestStore;
    private readonly ILogger<PasswordResetManager> _logger;

    public PasswordResetManager(IUserManager userManager, IMailTemplateStore mailTemplateStore, IMailGunService mailGunService, IPasswordResetRequestStore passwordResetRequestStore, ILogger<PasswordResetManager> logger)
    {
        _userManager = userManager;
        _mailTemplateStore = mailTemplateStore;
        _mailGunService = mailGunService;
        _passwordResetRequestStore = passwordResetRequestStore;
        _logger = logger;
    }

    public async Task<bool> InitiatePasswordReset(UserEntity user, CancellationToken cancellationToken)
    {
        string token = StringUtils.GenerateUrlSafeRandomString(16);
        string tokenHash = HashingUtils.Sha256_String(token);

        await _passwordResetRequestStore.UpsertAsync(user.Id, tokenHash, cancellationToken);

        string? emailTemplate = await _mailTemplateStore.GetTemplateAsync(EmailTemplateNames.PasswordReset, cancellationToken);
        if (emailTemplate == null) return false;

        string formattedEmail = new QuickStringReplacer(emailTemplate)
            .Replace("{{UserName}}", user.Name)
            .Replace("{{ResetPasswordUrl}}", App.WebsiteUrl + "/reset-password?token=" + token)
            .Replace("{{CompanyName}}", App.AppCreator)
            .Replace("{{CompanyAddress}}", App.MadeInText)
            .Replace("{{PoweredBy}}", App.AppName)
            .Replace("{{PoweredByLink}}", App.WebsiteUrl)
            .ToString();

        // Send recovery secret to email
        return await _mailGunService.SendEmailAsync("Hello", user.Name, user.Email, "Password recovery", formattedEmail, cancellationToken);
    }

    public async Task<bool> InitiatePasswordReset(Guid accountId, CancellationToken cancellationToken)
    {
        UserEntity? userEntity = await _userManager.Store.GetByIdAsync(accountId, cancellationToken);
        if (userEntity == null) return false;

        return await InitiatePasswordReset(userEntity, cancellationToken);
    }

    public async Task<bool> InitiatePasswordReset(string accountEmail, CancellationToken cancellationToken)
    {
        UserEntity? userEntity = await _userManager.Store.GetByEmailAsync(accountEmail, cancellationToken);
        if (userEntity == null) return false;

        return await InitiatePasswordReset(userEntity, cancellationToken);
    }

    public async Task<bool> TryCompletePasswordReset(string token, string newPassword, CancellationToken cancellationToken)
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
        return await _userManager.SetPasswordAsync(passwordResetRequest.UserId, newPassword, cancellationToken);
    }

    public Task<int> RemoveExpiredRequests(CancellationToken cancellationToken)
    {
        return _passwordResetRequestStore.DeleteExpiredAsync(TimeSpan.FromMinutes(30), cancellationToken);
    }
}
