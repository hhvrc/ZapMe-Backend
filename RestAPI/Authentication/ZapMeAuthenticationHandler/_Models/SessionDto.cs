using ZapMe.Database.Models;

namespace ZapMe.Authentication.Models;

public sealed class SessionDto
{
    public SessionDto(SessionEntity session)
    {
        SessionToken = session.Id;
        IssuedAtUtc = session.CreatedAt;
        ExpiresAtUtc = session.ExpiresAt;
    }

    /// <summary>
    /// 
    /// </summary>
    public Guid SessionToken { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime IssuedAtUtc { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime ExpiresAtUtc { get; init; }
}
