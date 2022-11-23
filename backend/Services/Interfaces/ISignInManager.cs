using ZapMe.Data.Models;
using ZapMe.DTOs;

namespace ZapMe.Services.Interfaces;

public interface ISignInManager
{
    /// <summary>
    /// 
    /// </summary>
    ISignInStore SignInStore { get; }

    /// <summary>
    /// 
    /// </summary>
    ILockOutManager LockOutManager { get; }

    /// <summary>
    /// 
    /// </summary>
    IUserManager UserManager { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signInId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SignInEntity?> TryGetSignInAsync(Guid signInId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signInId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsValidAsync(Guid signInId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsSignedInAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsLockedOutAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SignInResult> PasswordSignInAsync(AccountEntity user, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SignInResult> PasswordSignInAsync(Guid userId, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SignInResult> PasswordSignInAsync(string userName, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signInId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RefreshSignInAsync(Guid signInId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="signInId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SignOutAsync(Guid signInId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SignOutAllAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="reason"></param>
    /// <param name="flags"></param>
    /// <param name="expiresAt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task LockoutAsync(Guid userId, string? reason = null, string flags = "", DateTime? expiresAt = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ResetLockoutAsync(Guid userId, CancellationToken cancellationToken = default);
}
