using Microsoft.IdentityModel.Tokens;
using OneOf;
using System.Security.Claims;
using ZapMe.Database.Models;
using ZapMe.Enums.Errors;

namespace ZapMe.Services.Interfaces;

public interface IJwtAuthenticationManager
{

    Task<OneOf<SessionEntity, JwtAuthenticationError>> AuthenticateJwtTokenAsync(string jwtToken, CancellationToken cancellationToken = default);
    bool ValidateJwtToken(string jwtToken, out ClaimsPrincipal claimsPrincipal, out SecurityToken validatedToken);
    string GenerateJwtToken(ClaimsIdentity claimsIdentity, DateTime issuedAt, DateTime expiresAt);
}