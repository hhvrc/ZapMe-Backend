using ZapMe.DTOs;
using ZapMe.Enums.Errors;

namespace ZapMe.Mappers;

public static class CreateWebSocketErrorMapper
{
    public static ErrorDetails MapToErrorDetails(CreateWebSocketError createWebSocketError)
    {
        return createWebSocketError switch
        {
            CreateWebSocketError.InvalidClientMessage => throw new NotImplementedException(),
            CreateWebSocketError.InvalidClientJwt => throw new NotImplementedException(),
            CreateWebSocketError.ClientEmailUnverified => throw new NotImplementedException(),
            CreateWebSocketError.InvalidClientSession => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(createWebSocketError), createWebSocketError, null)
        };
    }
}
