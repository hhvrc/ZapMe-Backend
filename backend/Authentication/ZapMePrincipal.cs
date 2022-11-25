using System.Security.Claims;
using ZapMe.Data.Models;

namespace ZapMe.Authentication;

public class ZapMePrincipal : ClaimsPrincipal
{
    public ZapMePrincipal(AccountEntity account) : base(new ZapMeIdentity(account))
    {
    }

    public new ZapMeIdentity Identity => (base.Identity as ZapMeIdentity)!;
}
