using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class SessionManager : ISessionManager
{
    public ISessionStore SignInStore { get; }
    private readonly ILogger<SessionManager> _logger;

    public SessionManager(ISessionStore sessionStore, ILogger<SessionManager> logger)
    {
        SignInStore = sessionStore;
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task<SessionEntity?> GetSignInAsync(Guid sessionId, CancellationToken cancellationToken) => SignInStore.GetByIdAsync(sessionId, cancellationToken);

    /// <inheritdoc/>
    public Task<SessionEntity?> SignInAsync(Guid userId, string deviceName, TimeSpan expiresIn, CancellationToken cancellationToken) => SignInStore.TryCreateAsync(userId, deviceName, DateTime.UtcNow + expiresIn, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> RefreshSignInAsync(Guid sessionId, TimeSpan expiresIn, CancellationToken cancellationToken) => SignInStore.SetExipresAtAsync(sessionId, DateTime.UtcNow + expiresIn, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> SignOutAsync(Guid sessionId, CancellationToken cancellationToken) => SignInStore.DeleteAsync(sessionId, cancellationToken);

    /// <inheritdoc/>
    public Task<int> SignOutAllAsync(Guid userId, CancellationToken cancellationToken) => SignInStore.DeleteAllAsync(userId, cancellationToken);

    /// <inheritdoc/>
    public async Task<bool> IsValidAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        SessionEntity? session = await SignInStore.GetByIdAsync(sessionId, cancellationToken);
        if (session is null)
        {
            return false;
        }

        if (session.IsExpired)
        {
            await SignInStore.DeleteAsync(session.Id, cancellationToken);
            return false;
        }

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> IsSignedInAsync(Guid userId, CancellationToken cancellationToken)
    {
        SessionEntity[] sessions = await SignInStore.ListByUserAsync(userId, cancellationToken);

        return sessions.Any(session => !session.IsExpired);
    }
}
