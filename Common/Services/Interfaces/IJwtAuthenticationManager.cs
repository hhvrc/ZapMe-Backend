using Microsoft.IdentityModel.Tokens;
using OneOf;
using System.Security.Claims;
using ZapMe.Database.Models;

namespace ZapMe.Services.Interfaces;

public interface IJwtAuthenticationManager
{
    public enum AuthenticationError
    {
        InvalidToken,
        UnverifiedEmail,
        InvalidSession,
    }

    Task<OneOf<SessionEntity, AuthenticationError>> AuthenticateJwtTokenAsync(string jwtToken, CancellationToken cancellationToken = default);
    bool ValidateJwtToken(string jwtToken, out ClaimsPrincipal claimsPrincipal, out SecurityToken validatedToken);
    string GenerateJwtToken(ClaimsIdentity claimsIdentity, DateTime issuedAt, DateTime expiresAt);
}