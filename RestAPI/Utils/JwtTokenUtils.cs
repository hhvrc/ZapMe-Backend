using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZapMe.Constants;
using ZapMe.Database.Models;
using ZapMe.DTOs;

namespace ZapMe.Utils;

public static class JwtTokenUtils
{
    public static string GenerateJwtToken(IEnumerable<Claim> claims, DateTime expiresAt, string jwtSecret)
    {
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentException.ThrowIfNullOrEmpty(jwtSecret);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
                       issuer: AuthenticationConstants.JwtIssuer,
                       audience: AuthenticationConstants.JwtAudience,
                       claims: claims,
                       expires: expiresAt,
                       signingCredentials: credentials
                    );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public static string GenerateJwtToken(ClaimsIdentity claimsIdentity, DateTime expiresAt, string jwtSecret)
    {
        ArgumentNullException.ThrowIfNull(claimsIdentity);
        return GenerateJwtToken(claimsIdentity.Claims, expiresAt, jwtSecret);
    }
    public static string GenerateJwtToken(ClaimsPrincipal claimsPrincipal, DateTime expiresAt, string jwtSecret)
    {
        ArgumentNullException.ThrowIfNull(claimsPrincipal);
        return GenerateJwtToken(claimsPrincipal.Claims, expiresAt, jwtSecret);
    }
    public static string GenerateJwtToken(SessionEntity session, string jwtSecret)
    {
        ArgumentNullException.ThrowIfNull(session);
        return GenerateJwtToken(session.ToClaimsIdentity(), session.ExpiresAt, jwtSecret);
    }

    public static bool ValidateJwtToken(string jwtToken, string jwtSecret, out ClaimsPrincipal claimsPrincipal, out SecurityToken validatedToken)
    {
        ArgumentNullException.ThrowIfNull(jwtToken);
        ArgumentException.ThrowIfNullOrEmpty(jwtSecret);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSecret);

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
                ClockSkew = TimeSpan.Zero
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
}
