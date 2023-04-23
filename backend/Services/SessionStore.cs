using Microsoft.EntityFrameworkCore;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class SessionStore : ISessionStore
{
    private readonly ZapMeContext _dbContext;
    private readonly ILogger<SessionStore> _logger;

    public SessionStore(ZapMeContext dbContext, ILogger<SessionStore> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<SessionEntity> CreateAsync(UserEntity user, string? sessionName, string ipAddress, string countryCode, UserAgentEntity userAgent, DateTime expiresAt, CancellationToken cancellationToken)
    {
        SessionEntity session = new SessionEntity
        {
            User = null!,
            UserId = user.Id,
            Name = sessionName,
            IpAddress = ipAddress,
            CountryCode = countryCode,
            UserAgentId = userAgent.Id,
            UserAgent = null!,
            ExpiresAt = expiresAt
        };

        await _dbContext.Sessions.AddAsync(session, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return session;
    }

    public async Task<bool> SetExipresAtAsync(Guid sessionId, DateTime expiresAt, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions.Where(s => s.Id == sessionId).ExecuteUpdateAsync(s => s.SetProperty(s => s.ExpiresAt, _ => expiresAt), cancellationToken) > 0;
    }

    public async Task<bool> DeleteSessionAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        return await _dbContext.Sessions.Where(s => s.Id == sessionId).ExecuteDeleteAsync(cancellationToken) > 0;
    }

    public Task<int> DeleteUserSessionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _dbContext.Sessions.Where(s => s.UserId == userId).ExecuteDeleteAsync(cancellationToken);
    }
}
