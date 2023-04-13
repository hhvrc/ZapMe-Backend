using ZapMe.Data.Models;
using ZapMe.DTOs;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;

namespace ZapMe.Services;

public sealed class UserManager : IUserManager
{
    public IUserStore Store { get; }
    private readonly ILogger<UserManager> _logger;

    public UserManager(IUserStore userStore, ILogger<UserManager> logger)
    {
        Store = userStore;
        _logger = logger;
    }

    public Task<AccountCreationResult> TryCreateAsync(string userName, string email, string password, CancellationToken cancellationToken)
    {
        string passwordHash = PasswordUtils.HashPassword(password);

        return Store.TryCreateAsync(userName.TrimAndMinifyWhiteSpaces(), email, passwordHash, cancellationToken);
    }

    public Task<bool> SetPasswordAsync(Guid userId, string password, CancellationToken cancellationToken)
    {
        string passwordHash = PasswordUtils.HashPassword(password);
        return Store.SetPasswordHashAsync(userId, passwordHash, cancellationToken);
    }

    public async Task<PasswordCheckResult> CheckPasswordAsync(Guid userId, string password, CancellationToken cancellationToken)
    {
        UserEntity? user = await Store.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return PasswordCheckResult.UserNotFound;
        }

        if (!PasswordUtils.CheckPassword(password, user.PasswordHash))
        {
            return PasswordCheckResult.PasswordInvalid;
        }

        return PasswordCheckResult.Success;
    }
}