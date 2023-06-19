using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ZapMe.Database;
using ZapMe.Database.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class SessionStore : ISessionStore
{
    private readonly IDistributedCache _cache;
    private readonly DatabaseContext _dbContext;
    private readonly ILogger<SessionStore> _logger;

    public SessionStore(IDistributedCache cache, DatabaseContext dbContext, ILogger<SessionStore> logger)
    {
        _cache = cache;
        _dbContext = dbContext;
        _logger = logger;
    }

    private Task SetCacheAsync(string sessionKey, SessionEntity? session, CancellationToken cancellationToken)
    {
        if (session is null)
        {
            return _cache.RemoveAsync(sessionKey, cancellationToken);
        }
        else
        {
            return _cache.SetAsync(sessionKey, session, session.ExpiresAt, cancellationToken);
        }
    }

    public async Task<SessionEntity> CreateAsync(Guid userId, string ipAddress, string countryCode, Guid userAgentId, DateTime expiresAt, CancellationToken cancellationToken)
    {
        SessionEntity session = new SessionEntity()
        {
            UserId = userId,
            IpAddress = ipAddress,
            CountryCode = countryCode,
            UserAgentId = userAgentId,
            ExpiresAt = expiresAt
        };

        await _dbContext.Sessions.AddAsync(session, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        session = await _dbContext
            .Sessions
            .Include(s => s.User)
            .Include(s => s.UserAgent)
            .FirstAsync(s => s.Id == session.Id, cancellationToken);

        //string sessionKey = RedisCachePrefixes.Session + session.Id.ToString();
        //await SetCacheAsync(sessionKey, session, cancellationToken);

        return session;
    }

    public async Task<SessionEntity?> TryGetAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        /*
        string sessionKey = RedisCachePrefixes.Session + sessionId.ToString();

        SessionEntity? session = await _cache.GetAsync<SessionEntity>(sessionKey, cancellationToken);
        if (session is not null)
        {
            return session;
        }
        */

        SessionEntity? session = await _dbContext
            .Sessions
            .Include(s => s.User)
            .Include(s => s.UserAgent)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);

        //await SetCacheAsync(sessionKey, session, cancellationToken);

        return session;
    }
}
