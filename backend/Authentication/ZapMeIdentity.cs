using System.Security.Claims;
using ZapMe.Data.Models;

namespace ZapMe.Authentication;

public class ZapMeIdentity : ClaimsIdentity
{
    public ZapMeIdentity(SessionEntity session) : base(ZapMeAuthenticationDefaults.AuthenticationScheme)
    {
        Session = session;

        ICollection<UserRoleEntity>? roles = session.Account.UserRoles;

        // Add Role Claims
        if (roles != null)
        {
            foreach (UserRoleEntity userRole in roles)
            {
                AddClaim(new Claim(ClaimTypes.Role, userRole.RoleName, ClaimValueTypes.String, ZapMeAuthenticationDefaults.AuthenticationScheme, ZapMeAuthenticationDefaults.AuthenticationScheme, this));
            }
        }
    }

    public SessionEntity Session { get; }
    public Guid SessionId => Session.Id;
    public AccountEntity Account => Session.Account;
    public Guid AccountId => Account.Id;
    public IEnumerable<string> Roles => Account.UserRoles?.Select(x => x.RoleName) ?? Enumerable.Empty<string>();
}

