using ZapMe.Data.Models;
using ZapMe.DTOs;

namespace ZapMe.Services.Interfaces;

public interface IAccountManager
{
    /// <summary>
    /// 
    /// </summary>
    IAccountStore AccountStore { get; }

    /// <summary>
    /// 
    /// </summary>
    IPasswordHasher PasswordHasher { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccountCreationResult> TryCreateAsync(string name, string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAsync(Guid accountId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccountEntity?> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccountEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

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
    /// <param name="nameOrEmail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccountEntity?> GetByNameOrEmail(string nameOrEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetNameAsync(Guid accountId, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="email"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetEmailAsync(Guid accountId, string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetPasswordAsync(Guid accountId, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="lastOnline"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetLastOnlineAsync(Guid accountId, DateTime lastOnline, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="account"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    PasswordCheckResult CheckPassword(in AccountEntity account, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PasswordCheckResult> CheckPasswordAsync(Guid accountId, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string?> GeneratePasswordResetTokenAsync(Guid accountId, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="passwordResetToken"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> TryCompletePasswordResetAsync(string passwordResetToken, string password, CancellationToken cancellationToken = default);
}