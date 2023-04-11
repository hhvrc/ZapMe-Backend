using ZapMe.Data.Models;
using ZapMe.DTOs;

namespace ZapMe.Services.Interfaces;

public interface IAccountManager
{
    /// <summary>
    /// 
    /// </summary>
    IAccountStore Store { get; }

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
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetPasswordAsync(Guid accountId, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="password"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PasswordCheckResult> CheckPasswordAsync(Guid accountId, string password, CancellationToken cancellationToken = default);
}