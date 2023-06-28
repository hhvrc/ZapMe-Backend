using System.Security.Claims;
using ZapMe.Constants;
using ZapMe.Database.Models;

namespace ZapMe.DTOs;

public static class SessionMapper
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
        claimsIdentity.AddClaims(session.User.UserRoles?.Select(r => new Claim(ClaimTypes.Role, r.RoleName)) ?? Enumerable.Empty<Claim>());

        // Add Avatar URL Claim
        if (session.User.ProfileAvatar is not null)
        {
            claimsIdentity.AddClaim(new Claim(ZapMeClaimTypes.UserAvatarUrl, session.User.ProfileAvatar.PublicUrl.ToString()));
        }

        // Add Banner URL Claim
        if (session.User.ProfileBanner is not null)
        {
            claimsIdentity.AddClaim(new Claim(ZapMeClaimTypes.UserBannerUrl, session.User.ProfileBanner.PublicUrl.ToString()));
        }

        return claimsIdentity;
    }
}
