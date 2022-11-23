using ZapMe.Data.Models;
using ZapMe.DTOs;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class SignInManager : ISignInManager
{
    public ISignInStore SignInStore { get; }
    public ILockOutManager LockOutManager { get; }
    public IUserManager UserManager { get; }
    private readonly ILogger<SignInManager> _logger;

    public SignInManager(ISignInStore signInStore, ILockOutManager lockOutManager, IUserManager userManager, ILogger<SignInManager> logger)
    {
        SignInStore = signInStore;
        LockOutManager = lockOutManager;
        UserManager = userManager;
        _logger = logger;
    }

    public async Task<SignInEntity?> TryGetSignInAsync(Guid signInId, CancellationToken cancellationToken)
    {
        return await SignInStore.GetByIdAsync(signInId, cancellationToken);
    }

    public async Task<bool> IsValidAsync(Guid signInId, CancellationToken cancellationToken)
    {
        return (await SignInStore.GetByIdAsync(signInId, cancellationToken))?.IsValid ?? false;
    }

    public async Task<bool> IsSignedInAsync(Guid userId, CancellationToken cancellationToken)
    {
        return (await SignInStore.ListByUserAsync(userId, cancellationToken))?.Where(s => s.ExpiresAt < DateTime.UtcNow).Any() ?? false;
    }

    public Task<bool> IsLockedOutAsync(Guid userId, CancellationToken cancellationToken)
    {
        return LockOutManager.IsLockedOutAsync(userId, cancellationToken);
    }

    public async Task<SignInResult> PasswordSignInAsync(AccountEntity user, string password, CancellationToken cancellationToken)
    {
        if (password == null) throw new NullReferenceException(nameof(password));

        if (await LockOutManager.IsLockedOutAsync(user.Id, cancellationToken))
        {
            return SignInResultType.LockedOut;
        }

        var result = UserManager.CheckPassword(in user, password, cancellationToken);
        if (result != PasswordCheckResult.Success)
        {
            return result switch
            {
                PasswordCheckResult.UserNotFound => SignInResultType.UserNotFound,
                PasswordCheckResult.PasswordInvalid => SignInResultType.PasswordInvalid,
                _ => throw new NotImplementedException($"Value: {result}")
            };
        }

        var signIn = await SignInStore.TryCreateAsync(user.Id, "a", DateTime.UtcNow + TimeSpan.FromMinutes(30), cancellationToken);
        if (signIn == null)
        {
            _logger.LogCritical("SignIn is null!");
            return SignInResultType.InternalServerError;
        }

        return SignInResult.Success(signIn);
    }

    public async Task<SignInResult> PasswordSignInAsync(Guid userId, string password, CancellationToken cancellationToken)
    {
        if (password == null) throw new NullReferenceException(nameof(password));

        var user = await UserManager.GetByIdAsync(userId, cancellationToken);
        if (user == null) return SignInResultType.UserNotFound;

        return await PasswordSignInAsync(user, password, cancellationToken);
    }

    public async Task<SignInResult> PasswordSignInAsync(string userName, string password, CancellationToken cancellationToken)
    {
        if (userName == null) throw new NullReferenceException(nameof(userName));
        if (password == null) throw new NullReferenceException(nameof(password));

        var user = await UserManager.GetByNameAsync(userName, cancellationToken);
        if (user == null) return SignInResultType.UserNotFound;

        return await PasswordSignInAsync(user, password, cancellationToken);
    }

    public async Task RefreshSignInAsync(Guid signInId, CancellationToken cancellationToken)
    {
        await SignInStore.SetExipresAtAsync(signInId, DateTime.UtcNow + TimeSpan.FromMinutes(30), cancellationToken);
    }

    public Task SignOutAsync(Guid signInId, CancellationToken cancellationToken) => SignInStore.DeleteAsync(signInId, cancellationToken);
    public Task SignOutAllAsync(Guid userId, CancellationToken cancellationToken) => SignInStore.DeleteAllAsync(userId, cancellationToken);

    public Task LockoutAsync(Guid userId, string? reason, string flags, DateTime? expiresAt, CancellationToken cancellationToken) => LockOutManager.LockOutAsync(userId, reason, flags, expiresAt, cancellationToken);

    public Task ResetLockoutAsync(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
