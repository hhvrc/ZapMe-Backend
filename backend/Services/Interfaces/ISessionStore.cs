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
    /// <param name="user"></param>
    /// <param name="ipAddress"></param>
    /// <param name="countryCode"></param>
    /// <param name="userAgent"></param>
    /// <param name="expiresAt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SessionEntity> CreateAsync(UserEntity user, string ipAddress, string countryCode, UserAgentEntity userAgent, DateTime expiresAt, CancellationToken cancellationToken = default);
}
