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
    Task<LockOutEntity> CreateAsync(Guid userId, string? reason = null, string flags = "", DateTime? expiresAt = null, CancellationToken cancellationToken = default);
}
