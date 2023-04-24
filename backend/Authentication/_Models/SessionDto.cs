using ZapMe.Data.Models;

namespace ZapMe.Authentication.Models;

public sealed class SessionDto
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
    public Guid Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime IssuedAtUtc { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime ExpiresAtUtc { get; set; }
}
