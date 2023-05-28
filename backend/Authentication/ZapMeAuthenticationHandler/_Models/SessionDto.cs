using ZapMe.Data.Models;

namespace ZapMe.Authentication.Models;

public readonly struct SessionDto
{
    public SessionDto(SessionEntity session)
    {
        Id = session.Id;
        IssuedAtUtc = session.CreatedAt;
        ExpiresAtUtc = session.ExpiresAt;
    }

    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime IssuedAtUtc { get; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime ExpiresAtUtc { get; }
}
