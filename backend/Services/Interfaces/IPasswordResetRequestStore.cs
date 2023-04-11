using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

public interface IPasswordResetRequestStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<PasswordResetRequestEntity?> UpsertAsync(Guid accountId, string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<PasswordResetRequestEntity?> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<PasswordResetRequestEntity?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<PasswordResetRequestEntity?> TakeByTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> DeleteByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> DeleteByTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="maxAge"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<int> DeleteExpiredAsync(TimeSpan maxAge, CancellationToken cancellationToken = default);
}
