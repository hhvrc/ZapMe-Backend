using System.Security.Claims;
using ZapMe.Data.Models;

namespace ZapMe.Authentication;

public class ZapMeIdentity : ClaimsIdentity
{
    private Claim CreateClaim(string type, string value, string valueType) => new Claim(type, value, valueType, ZapMeAuthenticationDefaults.AuthenticationScheme, ZapMeAuthenticationDefaults.AuthenticationScheme, this);
    private Claim CreateClaim(string type, string value) => CreateClaim(type, value, ClaimValueTypes.String);

    private IEnumerable<Claim?> CreateClaims(AccountEntity account)
    {
        yield return CreateClaim(ClaimTypes.NameIdentifier, account.Id.ToString());
        yield return CreateClaim(ClaimTypes.Name, account.Username);
        yield return CreateClaim(ClaimTypes.Email, account.Email);

        ICollection<UserRoleEntity>? roles = account.UserRoles;
        if (roles != null)
        {
            foreach (UserRoleEntity role in roles)
            {
                yield return CreateClaim(ClaimTypes.Role, role.RoleName);
            }
        }
    }

    public ZapMeIdentity(AccountEntity account)
    {
        AddClaims(CreateClaims(account));
    }

    public string UserIdClaimType => ClaimTypes.NameIdentifier;
    private Guid? _userId = null;
    public Guid UserId
    {
        get
        {
            _userId ??= Guid.Parse(FindFirst(UserIdClaimType)!.Value);
            return _userId.Value;
        }
    }

    public SignInProperties? SignInProperties { get; set; }
}

