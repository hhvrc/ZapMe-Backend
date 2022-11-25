using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

/// <summary>
/// Represents a store for sign in entities.
/// </summary>
public interface ISessionStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="sessionName"></param>
    /// <param name="expiresAt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SessionEntity?> TryCreateAsync(Guid userId, string sessionName, DateTime expiresAt, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SessionEntity?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SessionEntity[]> ListByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="expiresAt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetExipresAtAsync(Guid sessionId, DateTime expiresAt, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Guid sessionId, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> DeleteAllAsync(Guid userId, CancellationToken cancellationToken);
}
