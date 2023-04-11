﻿using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

public interface IPasswordResetRequestStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="tokenHash"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<PasswordResetRequestEntity> UpsertAsync(Guid accountId, string tokenHash, CancellationToken cancellationToken = default);

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
    /// <param name="tokenHash"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<PasswordResetRequestEntity?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

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
    /// <param name="tokenHash"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> DeleteByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="maxAge"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<int> DeleteExpiredAsync(TimeSpan maxAge, CancellationToken cancellationToken = default);
}
