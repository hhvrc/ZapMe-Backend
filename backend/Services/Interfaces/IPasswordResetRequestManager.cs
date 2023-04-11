using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

public interface IPasswordResetRequestManager
{
    /// <summary>
    /// Will initiate a password request:
    /// <para>1. Generates a password reset token</para>
    /// <para>2. Hashes the generated reset token</para>
    /// <para>3. Inserts or Updates the database with the token hash</para>
    /// <para>4. Sends a mail to the accounts mail address containing a link to reset the password</para>
    /// </summary>
    /// <param name="account"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> InitiatePasswordReset(AccountEntity account, CancellationToken cancellationToken = default);

    /// <summary>
    /// Helper method for <see cref="InitiatePasswordReset(AccountEntity, CancellationToken)"/>
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> InitiatePasswordReset(Guid accountId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Helper method for <see cref="InitiatePasswordReset(AccountEntity, CancellationToken)"/>
    /// </summary>
    /// <param name="accountEmail"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> InitiatePasswordReset(string accountEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="newPassword"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> CompletePasswordReset(string token, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<int> RemoveExpiredRequests(CancellationToken cancellationToken = default);
}
