using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

public interface ISessionManager
{
    /// <summary>
    /// 
    /// </summary>
    ISessionStore SignInStore { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SessionEntity?> GetSignInAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="deviceName"></param>
    /// <param name="expiresIn"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SessionEntity?> SignInAsync(Guid userId, string deviceName, TimeSpan expiresIn, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="expiresIn"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> RefreshSignInAsync(Guid sessionId, TimeSpan expiresIn, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SignOutAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> SignOutAllAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsValidAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsSignedInAsync(Guid userId, CancellationToken cancellationToken = default);
}
