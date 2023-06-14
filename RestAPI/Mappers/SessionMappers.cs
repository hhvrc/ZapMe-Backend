using System.Security.Claims;
using ZapMe.Constants;
using ZapMe.Database.Models;

namespace ZapMe.DTOs;

public static class SessionMappers
{
    public static ClaimsIdentity ToClaimsIdentity(this SessionEntity session)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(session.User);

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(AuthenticationConstants.ZapMeScheme);

        claimsIdentity.AddClaim(new Claim(ZapMeClaimTypes.SessionId, session.Id.ToString()));
        claimsIdentity.AddClaim(new Claim(ZapMeClaimTypes.UserId, session.UserId.ToString()));
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
        if (session.User.UserRoles is not null)
        {
            claimsIdentity.AddClaims(session.User.UserRoles.Select(r => new Claim(ClaimTypes.Role, r.RoleName)));
        }

        return claimsIdentity;
    }
}
