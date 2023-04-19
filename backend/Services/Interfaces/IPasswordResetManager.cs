using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

public interface IPasswordResetManager
{
    /// <summary>
    /// Will initiate a password request:
    /// <para>1. Generates a password reset token</para>
    /// <para>2. Hashes the generated reset token</para>
    /// <para>3. Inserts or Updates the database with the token hash</para>
    /// <para>4. Sends a mail to the accounts mail address containing a link to reset the password</para>
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> InitiatePasswordReset(UserEntity user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Helper method for <see cref="InitiatePasswordReset(UserEntity, CancellationToken)"/>
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> InitiatePasswordReset(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Helper method for <see cref="InitiatePasswordReset(UserEntity, CancellationToken)"/>
    /// </summary>
    /// <param name="userEmail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> InitiatePasswordReset(string userEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="newPassword"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> TryCompletePasswordReset(string token, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<int> RemoveExpiredRequests(CancellationToken cancellationToken = default);
}
