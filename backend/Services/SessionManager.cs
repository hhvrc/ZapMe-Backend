using Microsoft.EntityFrameworkCore;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class SessionManager : ISessionManager
{
    private readonly ZapMeContext _dbContext;
    public ISessionStore SessionStore { get; }
    public IUserAgentManager UserAgentManager { get; }
    private readonly ILogger<SessionManager> _logger;

    public SessionManager(ZapMeContext dbContext, ISessionStore sessionStore, IUserAgentManager userAgentManager, ILogger<SessionManager> logger)
    {
        _dbContext = dbContext;
        SessionStore = sessionStore;
        UserAgentManager = userAgentManager;
        _logger = logger;
    }

    public async Task<SessionEntity> CreateAsync(UserEntity user, string? sessionName, string ipAddress, string countryCode, string userAgent, bool rememberMe, CancellationToken cancellationToken = default)
    {
        UserAgentEntity userAgentEntity = await UserAgentManager.EnsureCreatedAsync(userAgent, cancellationToken);
        DateTime expiresAt = DateTime.UtcNow.Add(rememberMe ? TimeSpan.FromDays(30) : TimeSpan.FromHours(1));
        return await SessionStore.CreateAsync(user, sessionName, ipAddress, countryCode, userAgentEntity, expiresAt, cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> IsValidSessionAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        SessionEntity? session = _dbContext.Sessions.Where(s => s.Id == sessionId).FirstOrDefault();
        if (session == null)
        {
            return false;
        }

        if (session.IsExpired)
        {
            await _dbContext.Sessions.Where(s => s.Id == sessionId).ExecuteDeleteAsync(cancellationToken);
            return false;
        }

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<bool> UserHasSessionAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _dbContext.Sessions.Where(s => s.UserId == userId).AnyAsync(cancellationToken);
    }
}
