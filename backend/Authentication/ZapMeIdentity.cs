using System.Security.Claims;
using ZapMe.Data.Models;

namespace ZapMe.Authentication;

public sealed class ZapMeIdentity : ClaimsIdentity
{
    public ZapMeIdentity(SessionEntity session) : base(ZapMeAuthenticationDefaults.AuthenticationScheme)
    {
        Session = session;

        ICollection<UserRoleEntity>? roles = session.User?.UserRoles;

        // Add Role Claims
        if (roles != null)
        {
            AddClaims(roles.Select(r => new Claim(ClaimTypes.Role, r.RoleName)));
        }
    }

    public SessionEntity Session { get; }
    public Guid SessionId => Session.Id;
    public UserEntity User => Session.User ?? throw new NullReferenceException("Session must contain a user");
    public Guid UserId => User.Id;
    public IEnumerable<string> Roles => User?.UserRoles?.Select(x => x.RoleName) ?? Enumerable.Empty<string>();
}

