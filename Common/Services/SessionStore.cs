﻿using ZapMe.Database;
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

    public async Task<SessionEntity> CreateAsync(UserEntity user, string ipAddress, string countryCode, UserAgentEntity userAgent, DateTime expiresAt, CancellationToken cancellationToken)
    {
        SessionEntity session = new SessionEntity
        {
            User = null!,
            UserId = user.Id,
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
}