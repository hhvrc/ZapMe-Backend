using OneOf;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

public interface IOAuthConnectionManager
{
    public Task<OneOf<OAuthConnectionEntity, ErrorDetails>> GetOrCreateConnectionAsync(string name, string email, ImageEntity? profilePicture, string provider, string providerId, CancellationToken cancellationToken);
}