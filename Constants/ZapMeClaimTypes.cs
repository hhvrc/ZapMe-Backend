using System.Security.Claims;

namespace ZapMe.Constants;

public static class ZapMeClaimTypes
{
    public const string SessionId = "zapme.session_id";
    public const string UserId = ClaimTypes.NameIdentifier;
    public const string UserName = ClaimTypes.Name;
    public const string UserEmail = ClaimTypes.Email;
    public const string UserEmailVerified = "zapme.email_verified";
    public const string UserProfileImage = "zapme.profile_image";
    public const string UserProfileBanner = "zapme.profile_banner";
    public const string UserDiscordId = "zapme.discord_id";
    public const string UserGithubId = "zapme.github_id";
    public const string UserTwitterId = "zapme.twitter_id";
}
