using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

/// <summary>
/// 
/// </summary>
public interface ILockOutStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="reason"></param>
    /// <param name="flags"></param>
    /// <param name="expiresAt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<LockOutEntity?> CreateAsync(Guid userId, string? reason = null, string flags = "", DateTime? expiresAt = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lockOutId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<LockOutEntity?> GetByIdAsync(Guid lockOutId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    IAsyncEnumerable<LockOutEntity> ListByUserIdAsync(Guid userId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lockOutId"></param>
    /// <param name="reason"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetReasonAsync(Guid lockOutId, string reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lockOutId"></param>
    /// <param name="flags"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetFlagsAsync(Guid lockOutId, string flags, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lockOutId"></param>
    /// <param name="expiresAt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetExipresAtAsync(Guid lockOutId, DateTime expiresAt, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lockOutId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Guid lockOutId, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> DeleteAllAsync(Guid userId, CancellationToken cancellationToken);
}
