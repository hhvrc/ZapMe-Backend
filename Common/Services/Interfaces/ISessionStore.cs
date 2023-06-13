using ZapMe.Database.Models;

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
    /// <param name="ipAddress"></param>
    /// <param name="countryCode"></param>
    /// <param name="userAgentId"></param>
    /// <param name="expiresAt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SessionEntity> CreateAsync(Guid userId, string ipAddress, string countryCode, Guid userAgentId, DateTime expiresAt, CancellationToken cancellationToken = default);
}
