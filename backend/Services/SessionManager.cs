using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class SessionManager : ISessionManager
{
    public ISessionStore SessionStore { get; }
    public IUserAgentStore UserAgentStore { get; }
    private readonly ILogger<SessionManager> _logger;

    public SessionManager(ISessionStore sessionStore, IUserAgentStore userAgentStore, ILogger<SessionManager> logger)
    {
        SessionStore = sessionStore;
        UserAgentStore = userAgentStore;
        _logger = logger;
    }

    public async Task<SessionEntity> CreateAsync(AccountEntity account, string? sessionName, string ipAddress, string countryCode, string userAgent, bool rememberMe, CancellationToken cancellationToken = default)
    {
        UserAgentEntity userAgentEntity = await UserAgentStore.EnsureCreatedAsync(userAgent, cancellationToken);
        DateTime expiresAt = DateTime.UtcNow.Add(rememberMe ? TimeSpan.FromDays(30) : TimeSpan.FromHours(1));
        return await SessionStore.CreateAsync(account, sessionName, ipAddress, countryCode, userAgentEntity, expiresAt, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> IsValidSessionAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        SessionEntity? session = await SessionStore.GetByIdAsync(sessionId, cancellationToken);
        if (session == null)
        {
            return false;
        }

        if (session.IsExpired)
        {
            await SessionStore.DeleteSessionAsync(session.Id, cancellationToken);
            return false;
        }

        return true;
    }

    /// <inheritdoc/>
    public ValueTask<bool> UserHasSessionAsync(Guid userId, CancellationToken cancellationToken)
    {
        return SessionStore.ListByUserAsync(userId).AnyAsync(session => !session.IsExpired, cancellationToken);
    }
}
