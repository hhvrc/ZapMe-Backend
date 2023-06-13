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

    public async Task<SessionEntity> CreateAsync(Guid userId, string ipAddress, string countryCode, string userAgent, bool rememberMe, CancellationToken cancellationToken)
    {
        UserAgentEntity userAgentEntity = await UserAgentManager.EnsureCreatedAsync(userAgent, cancellationToken);
        DateTime expiresAt = DateTime.UtcNow.Add(rememberMe ? TimeSpan.FromDays(30) : TimeSpan.FromHours(1)); // TODO: Make this configurable
        return await SessionStore.CreateAsync(userId, ipAddress, countryCode, userAgentEntity.Id, expiresAt, cancellationToken);
    }
}
