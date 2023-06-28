using ZapMe.Enums.Errors;

namespace ZapMe.DTOs;

public static class JwtAuthenticationErrorMapper
{
    public static CreateWebSocketError MapToCreateWebSocketError(JwtAuthenticationError jwtAuthenticationError)
    {
        return jwtAuthenticationError switch
        {
            JwtAuthenticationError.InvalidToken => CreateWebSocketError.InvalidClientJwt,
            JwtAuthenticationError.UnverifiedEmail => CreateWebSocketError.ClientEmailUnverified,
            JwtAuthenticationError.InvalidSession => CreateWebSocketError.InvalidClientSession,
            _ => throw new ArgumentOutOfRangeException(nameof(jwtAuthenticationError), jwtAuthenticationError, null)
        };
    }
}
