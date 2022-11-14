using ZapMe.Data.Models;
using ZapMe.DTOs;
using ZapMe.Logic;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class UserManager : IUserManager
{
    public IUserStore UserStore { get; }
    public IPasswordHasher PasswordHasher { get; }
    private readonly ILogger<UserManager> _logger;

    public UserManager(IUserStore userStore, IPasswordHasher passwordHasher, ILogger<UserManager> logger)
    {
        UserStore = userStore;
        PasswordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<UserEntity?> TryCreateAsync(string userName, string email, string password, CancellationToken cancellationToken)
    {
        var passwordHash = PasswordHasher.HashPassword(password);

        return await UserStore.TryCreateAsync(userName, email, passwordHash, cancellationToken);
    }

    public Task DeleteAsync(Guid userId, CancellationToken cancellationToken) => UserStore.DeleteAsync(userId, cancellationToken);
    public Task<UserEntity?> GetByIdAsync(Guid userId, CancellationToken cancellationToken) => UserStore.GetByIdAsync(userId, cancellationToken);
    public Task<UserEntity?> GetByNameAsync(string userName, CancellationToken cancellationToken) => UserStore.GetByNameAsync(userName, cancellationToken);
    public Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken) => UserStore.GetByEmailAsync(email, cancellationToken);
    public Task<bool> SetUserNameAsync(Guid userId, string userName, CancellationToken cancellationToken) => UserStore.SetUserNameAsync(userId, userName, cancellationToken);
    public Task<bool> SetEmailAsync(Guid userId, string email, CancellationToken cancellationToken) => UserStore.SetEmailAsync(userId, email, cancellationToken);
    public Task<bool> SetPasswordAsync(Guid userId, string password, CancellationToken cancellationToken) => UserStore.SetPasswordHashAsync(userId, PasswordHasher.HashPassword(password), cancellationToken);
    public Task<bool> SetLastOnlineAsync(Guid userId, DateTime lastOnline, CancellationToken cancellationToken) => UserStore.SetLastOnlineAsync(userId, lastOnline, cancellationToken);

    public PasswordCheckResult CheckPassword(UserEntity user, string password, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user, nameof(user));

        if (!PasswordHasher.CheckPassword(password, user.PasswordHash))
        {
            return PasswordCheckResult.PasswordInvalid;
        }

        return PasswordCheckResult.Success;
    }

    public async Task<PasswordCheckResult> CheckPasswordAsync(Guid userId, string password, CancellationToken cancellationToken)
    {
        var user = await UserStore.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return PasswordCheckResult.UserNotFound;
        }

        return CheckPassword(user, password, cancellationToken);
    }

    public async Task<string?> GeneratePasswordResetTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        string token = Utils.GenerateRandomString(32);

        if (!await UserStore.SetPasswordResetTokenAsync(userId, token, cancellationToken))
        {
            return null;
        }

        return token;
    }

    public async Task<bool> TryCompletePasswordResetAsync(string passwordResetToken, string password, CancellationToken cancellationToken)
    {
        bool result = false;

        UserEntity? user = await UserStore.GetByPasswordResetTokenAsync(passwordResetToken, cancellationToken);
        if (user?.PasswordResetRequestedAt.HasValue ?? false)
        {
            TimeSpan tokenAge = DateTime.UtcNow - user.PasswordResetRequestedAt.Value;

            if (tokenAge < TimeSpan.FromHours(1))
            {
                result = await SetPasswordAsync(user.Id, password, cancellationToken);
            }
            else
            {
                _logger.LogWarning("Password reset token for user {} expired", user.Id);
            }

            await UserStore.SetPasswordResetTokenAsync(user.Id, null, cancellationToken);
        }
        else
        {
            _logger.LogWarning("Password reset token {} not found", passwordResetToken);
        }

        return result;
    }
}