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


    public async Task<SessionEntity?> TryCreateAsync(Guid userId, string sessionName, DateTime expiresAt, CancellationToken cancellationToken)
    {
        SessionEntity session = new SessionEntity
        {
            User = null!, // TODO: wtf do i do now? ask on C# discord
            UserId = userId,
            Name = sessionName,
            ExpiresAt = expiresAt
        };

        await _dbContext.Sessions.AddAsync(session, cancellationToken);
        int nAdded = await _dbContext.SaveChangesAsync(cancellationToken);

        if (nAdded > 0)
        {
            return await GetByIdAsync(session.Id, cancellationToken);
        }

        return null;
    }

    public Task<SessionEntity?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        return _dbContext.Sessions.Include(static s => s.User).FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);
    }

    public Task<SessionEntity[]> ListByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _dbContext.Sessions.Where(s => s.UserId == userId).ToArrayAsync(cancellationToken);
    }

    public async Task<bool> SetExipresAtAsync(Guid sessionId, DateTime expiresAt, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions.Where(s => s.Id == sessionId).ExecuteUpdateAsync(s => s.SetProperty(static s => s.ExpiresAt, _ => expiresAt), cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        return await _dbContext.Sessions.Where(s => s.Id == sessionId).ExecuteDeleteAsync(cancellationToken) > 0;
    }

    public Task<int> DeleteAllAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _dbContext.Sessions.Where(s => s.UserId == userId).ExecuteDeleteAsync(cancellationToken);
    }
}
