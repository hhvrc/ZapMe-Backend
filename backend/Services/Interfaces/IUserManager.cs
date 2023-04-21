using ZapMe.DTOs;

namespace ZapMe.Services.Interfaces;

public interface IUserManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="email">User's email, pass null to force the user to verify their email address after creation.</param>
    /// <param name="password">User's password, will be hashed with <see cref="Utils.PasswordUtils"/>.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AccountCreationResult> TryCreateAsync(string name, string? email, string password, CancellationToken cancellationToken = default);
}