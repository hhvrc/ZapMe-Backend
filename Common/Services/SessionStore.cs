using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class SessionStore : ISessionStore
{
    private readonly DatabaseContext _dbContext;
    private readonly ILogger<SessionStore> _logger;

    public SessionStore(DatabaseContext dbContext, ILogger<SessionStore> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<SessionEntity> CreateAsync(Guid userId, string ipAddress, string countryCode, Guid userAgentId, DateTime expiresAt, CancellationToken cancellationToken)
    {
        SessionEntity session = new SessionEntity
        {
            User = null!,
            UserId = userId,
            IpAddress = ipAddress,
            CountryCode = countryCode,
            UserAgentId = userAgentId,
            UserAgent = null!,
            ExpiresAt = expiresAt
        };

        await _dbContext.Sessions.AddAsync(session, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return session;
    }
}
