using System.Security.Claims;
using ZapMe.Constants;
using ZapMe.Database.Models;
using ZapMe.DTOs;

namespace ZapMe.DTOs;

public static class SessionMappers
{
    public static SessionDto ToDto(this SessionEntity session)
    {
        return new SessionDto
        {
            SessionToken = session.Id,
            IssuedAtUtc = session.CreatedAt,
            ExpiresAtUtc = session.ExpiresAt
        };
    }

    public static ClaimsIdentity ToClaimsIdentity(this SessionEntity session)
    {
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(AuthSchemes.Main);

        claimsIdentity.AddClaim(new Claim(ZapMeClaimTypes.SessionId, session.Id.ToString()));
        claimsIdentity.AddClaim(new Claim(ZapMeClaimTypes.UserId, session.UserId.ToString()));
        if (session.User is not null)
        {
            claimsIdentity.AddClaim(new Claim(ZapMeClaimTypes.UserName, session.User.Name));
            claimsIdentity.AddClaim(new Claim(ZapMeClaimTypes.UserEmail, session.User.Email));
            claimsIdentity.AddClaim(new Claim(ZapMeClaimTypes.UserEmailVerified, session.User.EmailVerified.ToString()));

            // Add Profile Picture URL Claim
            if (session.User.ProfilePicture is not null)
            {
                claimsIdentity.AddClaim(new Claim(ZapMeClaimTypes.UserProfileImage, session.User.ProfilePicture.PublicUrl.ToString()));
            }

            // Add Profile Banner URL Claim
            if (session.User.ProfileBanner is not null)
            {
                claimsIdentity.AddClaim(new Claim(ZapMeClaimTypes.UserProfileBanner, session.User.ProfileBanner.PublicUrl.ToString()));
            }

            // Add Role Claims
            if (session.User.UserRoles != null)
            {
                claimsIdentity.AddClaims(session.User.UserRoles.Select(r => new Claim(ClaimTypes.Role, r.RoleName)));
            }
        }

        return claimsIdentity;
    }
}
