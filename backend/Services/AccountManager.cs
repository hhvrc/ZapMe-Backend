using ZapMe.Data.Models;
using ZapMe.DTOs;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Services;

public sealed class AccountManager : IAccountManager
{
    public IAccountStore AccountStore { get; }
    private readonly ILogger<AccountManager> _logger;

    public AccountManager(IAccountStore userStore, ILogger<AccountManager> logger)
    {
        AccountStore = userStore;
        _logger = logger;
    }

    public Task<AccountCreationResult> TryCreateAsync(string userName, string email, string password, CancellationToken cancellationToken)
    {
        string passwordHash = PasswordUtils.HashPassword(password);

        return AccountStore.TryCreateAsync(userName.TrimAndMinifyWhiteSpaces(), email, passwordHash, cancellationToken);
    }

    public Task DeleteAsync(Guid userId, CancellationToken cancellationToken) => AccountStore.DeleteAsync(userId, cancellationToken);
    public Task<AccountEntity?> GetByIdAsync(Guid userId, CancellationToken cancellationToken) => AccountStore.GetByIdAsync(userId, cancellationToken);
    public Task<AccountEntity?> GetByNameAsync(string userName, CancellationToken cancellationToken) => AccountStore.GetByNameAsync(userName, cancellationToken);
    public Task<AccountEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken) => AccountStore.GetByEmailAsync(email, cancellationToken);
    public Task<AccountEntity?> GetByNameOrEmail(string userNameOrEmail, CancellationToken cancellationToken) => AccountStore.GetByUsernameOrEmail(userNameOrEmail, cancellationToken);
    public Task<bool> SetNameAsync(Guid userId, string userName, CancellationToken cancellationToken) => AccountStore.SetUserNameAsync(userId, userName, cancellationToken);
    public Task<bool> SetEmailAsync(Guid userId, string email, CancellationToken cancellationToken) => AccountStore.SetEmailAsync(userId, email, cancellationToken);
    public Task<bool> SetPasswordAsync(Guid userId, string password, CancellationToken cancellationToken) => AccountStore.SetPasswordHashAsync(userId, PasswordUtils.HashPassword(password), cancellationToken);
    public Task<bool> SetLastOnlineAsync(Guid userId, DateTime lastOnline, CancellationToken cancellationToken) => AccountStore.SetLastOnlineAsync(userId, lastOnline, cancellationToken);

    public PasswordCheckResult CheckPassword(in AccountEntity user, string password, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        if (!PasswordUtils.CheckPassword(password, user.PasswordHash))
        {
            return PasswordCheckResult.PasswordInvalid;
        }

        return PasswordCheckResult.Success;
    }

    public async Task<PasswordCheckResult> CheckPasswordAsync(Guid userId, string password, CancellationToken cancellationToken)
    {
        var user = await AccountStore.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return PasswordCheckResult.UserNotFound;
        }

        return CheckPassword(user, password, cancellationToken);
    }
}