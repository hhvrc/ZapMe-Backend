using ZapMe.Data.Models;
using ZapMe.DTOs;

namespace ZapMe.Services.Interfaces;

public interface IAccountStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="email"></param>
    /// <param name="passwordHash"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccountCreationResult> TryCreateAsync(string userName, string email, string passwordHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccountEntity?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccountEntity?> GetByNameAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccountEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userNameOrEmail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccountEntity?> GetByUsernameOrEmail(string userNameOrEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="passwordResetToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccountEntity?> GetByPasswordResetTokenAsync(string passwordResetToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetUserNameAsync(Guid userId, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="email"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetEmailAsync(Guid userId, string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="emailVerified"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetEmailVerifiedAsync(Guid userId, bool emailVerified, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="passwordHash"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetPasswordHashAsync(Guid userId, string passwordHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="lastOnline"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetLastOnlineAsync(Guid userId, DateTime lastOnline, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetPasswordResetTokenAsync(Guid userId, string? token, CancellationToken cancellationToken);
}
