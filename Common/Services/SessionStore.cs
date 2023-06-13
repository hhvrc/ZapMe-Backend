using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using ZapMe.Constants;
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

        string sessionKey = RedisCachePrefixes.Session + session.Id.ToString();

        await _cache.SetAsync(sessionKey, JsonSerializer.SerializeToUtf8Bytes(session), new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = session.ExpiresAt
        });

        return session;
    }

    public async Task<SessionEntity?> TryGetAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        string sessionKey = RedisCachePrefixes.Session + sessionId.ToString();

        var bytes = await _cache.GetAsync(sessionKey, cancellationToken);
        if (bytes is not null)
        {
            try
            {
                return JsonSerializer.Deserialize<SessionEntity>(bytes);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize session {SessionId}", sessionId);
            }
        }

        SessionEntity? session = await _dbContext.Sessions
            .Where(s => s.Id == sessionId)
            .Include(s => s.User)
            .Include(s => s.UserAgent)
            .SingleOrDefaultAsync(cancellationToken);

        if (session is not null)
        {
            await _cache.SetAsync(sessionKey, JsonSerializer.SerializeToUtf8Bytes(session), new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = session.ExpiresAt
            });
        }
        else if (bytes is not null)
        {
            // If we have corrupt data in the cache, remove it
            await _cache.RemoveAsync(sessionKey, cancellationToken);
        }

        return session;
    }
}
