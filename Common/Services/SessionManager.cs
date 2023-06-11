using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class SessionManager : ISessionManager
{
    private readonly DatabaseContext _dbContext;
    public ISessionStore SessionStore { get; }
    public IUserAgentManager UserAgentManager { get; }
    private readonly ILogger<SessionManager> _logger;

    public SessionManager(DatabaseContext dbContext, ISessionStore sessionStore, IUserAgentManager userAgentManager, ILogger<SessionManager> logger)
    {
        _dbContext = dbContext;
        SessionStore = sessionStore;
        UserAgentManager = userAgentManager;
        _logger = logger;
    }

    public async Task<SessionEntity> CreateAsync(UserEntity user, string ipAddress, string countryCode, string userAgent, bool rememberMe, CancellationToken cancellationToken)
    {
        UserAgentEntity userAgentEntity = await UserAgentManager.EnsureCreatedAsync(userAgent, cancellationToken);
        DateTime expiresAt = DateTime.UtcNow.Add(rememberMe ? TimeSpan.FromDays(30) : TimeSpan.FromHours(1));
        return await SessionStore.CreateAsync(user, ipAddress, countryCode, userAgentEntity, expiresAt, cancellationToken);
    }
}
