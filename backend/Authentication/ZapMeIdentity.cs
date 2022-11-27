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
    
    private Guid GetGuid(string claimType)
    {
        Claim? claim = FindFirst(claimType);

        if (claim == null)
        {
            throw new KeyNotFoundException($"Claim {claimType} not found.");
        }

        if (!Guid.TryParse(claim.Value, out Guid result))
        {
            throw new InvalidDataException($"Claim {claimType} is not a valid GUID");
        }

        return result;
    }

    public string SessionIdClaimType => ZapMeAuthenticationDefaults.AuthenticationScheme + ".SessionId";
    public Guid SessionId => GetGuid(SessionIdClaimType);

    public string UserIdClaimType => ClaimTypes.NameIdentifier;
    public Guid UserId => GetGuid(UserIdClaimType);

    public string UsernameClaimType => ClaimTypes.Name;
    public string Username => Name ?? throw new InvalidOperationException("Name cannot be null");

    public IEnumerable<string> Roles => FindAll(ClaimTypes.Role).Select(claim => claim.Value);

    public string SecurityStampClaimType => ZapMeAuthenticationDefaults.AuthenticationScheme + ".SecurityStamp";
    public Guid SecurityStamp => GetGuid(SecurityStampClaimType);
}

