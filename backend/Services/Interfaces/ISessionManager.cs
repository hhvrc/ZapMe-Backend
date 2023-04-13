using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

public interface ISessionManager
{
    /// <summary>
    /// 
    /// </summary>
    ISessionStore SessionStore { get; }

    Task<SessionEntity> CreateAsync(UserEntity user, string? sessionName, string ipAddress, string countryCode, string userAgent, bool rememberMe, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsValidSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<bool> UserHasSessionAsync(Guid userId, CancellationToken cancellationToken = default);
}
