using ZapMe.Database.Models;
using ZapMe.DTOs;

namespace ZapMe.DTOs;

public static class SessionMappers
{
    public static SessionDto ToDto(this SessionEntity entity)
    {
        return new SessionDto
        {
            SessionToken = entity.Id,
            IssuedAtUtc = entity.CreatedAt,
            ExpiresAtUtc = entity.ExpiresAt
        };
    }
}
