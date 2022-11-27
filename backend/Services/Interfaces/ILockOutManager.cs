using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

/// <summary>
/// 
/// </summary>
public interface ILockOutManager
{
    /// <summary>
    /// 
    /// </summary>
    ILockOutStore LockOutStore { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lockOutId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<LockOutEntity> GetAsync(Guid lockOutId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<LockOutEntity[]> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="reason"></param>
    /// <param name="flags"></param>
    /// <param name="expiresAt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task LockOutAsync(Guid userId, string? reason = null, string flags = "", DateTime? expiresAt = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsLockedOutAsync(Guid userId, CancellationToken cancellationToken = default);
}
