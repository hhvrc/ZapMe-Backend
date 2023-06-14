using ZapMe.Database.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class SessionManager : ISessionManager
{
    private readonly ISessionStore _sessionStore;
    private readonly IUserAgentManager _userAgentManager;
    private readonly ILogger<SessionManager> _logger;

    public SessionManager(ISessionStore sessionStore, IUserAgentManager userAgentManager, ILogger<SessionManager> logger)
    {
        _sessionStore = sessionStore;
        _userAgentManager = userAgentManager;
        _logger = logger;
    }

    public async Task<SessionEntity> CreateAsync(UserEntity user, string ipAddress, string countryCode, string userAgent, bool rememberMe, CancellationToken cancellationToken)
    {
        UserAgentEntity userAgentEntity = await _userAgentManager.EnsureCreatedAsync(userAgent, cancellationToken);
        DateTime expiresAt = DateTime.UtcNow.Add(rememberMe ? TimeSpan.FromDays(30) : TimeSpan.FromHours(1)); // TODO: Make this configurable

        SessionEntity session = await _sessionStore.CreateAsync(user.Id, ipAddress, countryCode, userAgentEntity.Id, expiresAt, cancellationToken);

        session.User = user;
        session.UserAgent = userAgentEntity;

        return session;
    }
}
