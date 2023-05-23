using System.Security.Claims;

namespace ZapMe.Authentication;

public static class ZapMeClaimTypes
{
    public const string Id = ClaimTypes.NameIdentifier;
    public const string Name = ClaimTypes.Name;
    public const string Email = ClaimTypes.Email;
    public const string ProfileImage = "zapme.profile_image";

}
