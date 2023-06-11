using ZapMe.Database.Models;

namespace ZapMe.Services.Interfaces;

public interface ISessionManager
{
    Task<SessionEntity> CreateAsync(UserEntity user, string ipAddress, string countryCode, string userAgent, bool rememberMe, CancellationToken cancellationToken = default);
}
