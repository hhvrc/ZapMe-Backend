using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OneOf;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZapMe.Constants;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Helpers;
using ZapMe.Options;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class JwtAuthenticationManager : IJwtAuthenticationManager
{
    private readonly ISessionStore _sessionStore;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<JwtAuthenticationManager> _logger;

    public JwtAuthenticationManager(ISessionStore sessionStore, IOptions<JwtOptions> jwtOptions, ILogger<JwtAuthenticationManager> logger)
    {
        _sessionStore = sessionStore;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<OneOf<SessionEntity, ErrorDetails>> AuthenticateJwtTokenAsync(string jwtToken, CancellationToken cancellationToken)
    {
        if (!ValidateJwtToken(jwtToken, out ClaimsPrincipal claimsPrincipal, out SecurityToken _))
        {
            return HttpErrors.Unauthorized;
        }

        if (!claimsPrincipal.GetUserEmailVerified())
        {
            return HttpErrors.UnverifiedEmail;
        }

        SessionEntity? session = await _sessionStore.TryGetAsync(claimsPrincipal.GetSessionId(), cancellationToken);
        if (session is null)
        {
            return HttpErrors.Unauthorized;
        }

        return session;
    }

    public bool ValidateJwtToken(string jwtToken, out ClaimsPrincipal claimsPrincipal, out SecurityToken validatedToken)
    {
        ArgumentNullException.ThrowIfNull(jwtToken);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtOptions.SigningKey);

        try
        {
            claimsPrincipal = tokenHandler.ValidateToken(jwtToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = AuthenticationConstants.JwtIssuer,
                ValidateAudience = true,
                ValidAudience = AuthenticationConstants.JwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(5),
            }, out validatedToken);
        }
        catch (Exception)
        {
            claimsPrincipal = null!;
            validatedToken = null!;
            return false;
        }

        return true;
    }

    public string GenerateJwtToken(ClaimsIdentity claimsIdentity, DateTime issuedAt, DateTime expiresAt)
    {
        ArgumentNullException.ThrowIfNull(claimsIdentity);

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            IssuedAt = issuedAt,
            Expires = expiresAt,
            SigningCredentials = credentials,
            Issuer = AuthenticationConstants.JwtIssuer,
            Audience = AuthenticationConstants.JwtAudience,
        };

        var securityToken = tokenHandler.CreateToken(descriptor);
        return tokenHandler.WriteToken(securityToken);
    }
}
