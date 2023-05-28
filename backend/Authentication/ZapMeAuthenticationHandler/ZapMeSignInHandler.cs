using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace ZapMe.Authentication;

public partial class ZapMeAuthenticationHandler
{
    public Task ZapMeSignInAsync(ClaimsPrincipal claimsIdentity, AuthenticationProperties? properties)
    {
        return FinishSignInAsync((claimsIdentity as ZapMePrincipal)!.Identity.Session);
    }
}