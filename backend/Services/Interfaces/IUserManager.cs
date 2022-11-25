using ZapMe.Data.Models;
using ZapMe.DTOs;

namespace ZapMe.Services.Interfaces;

public interface IUserManager
{
    /// <summary>
    /// 
    /// </summary>
    IUserStore UserStore { get; }

    /// <summary>
    /// 
    /// </summary>
    IPasswordHasher PasswordHasher { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccountEntity?> TryCreateAsync(string userName, string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default);

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
    Task<AccountEntity?> GetByUsernameAsync(string userName, CancellationToken cancellationToken = default);

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
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetPasswordAsync(Guid userId, string password, CancellationToken cancellationToken = default);

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
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    PasswordCheckResult CheckPassword(in AccountEntity user, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PasswordCheckResult> CheckPasswordAsync(Guid userId, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string?> GeneratePasswordResetTokenAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="passwordResetToken"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> TryCompletePasswordResetAsync(string passwordResetToken, string password, CancellationToken cancellationToken = default);
}