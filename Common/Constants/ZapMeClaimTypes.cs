using System.Security.Claims;

namespace ZapMe.Constants;

public static class ZapMeClaimTypes
{
    public const string SessionId = "zapme.session_id";
    public const string UserId = ClaimTypes.NameIdentifier;
    public const string UserName = ClaimTypes.Name;
    public const string UserEmail = ClaimTypes.Email;
    public const string UserEmailVerified = "zapme.email_verified";
    public const string UserAvatarUrl = "zapme.avatar_url";
    public const string UserBannerUrl = "zapme.banner_url";
    public const string UserDiscordId = "zapme.discord_id";
    public const string UserGithubId = "zapme.github_id";
    public const string UserTwitterId = "zapme.twitter_id";
}
