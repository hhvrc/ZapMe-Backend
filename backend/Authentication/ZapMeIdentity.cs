using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ZapMe.Data.Models;

namespace ZapMe.Authentication;

/*

        
        //create claims details based on the user information
        var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", user.UserId.ToString()),
                        new Claim("DisplayName", user.DisplayName),
                        new Claim("UserName", user.UserName),
                        new Claim("Email", user.Email)
                    };
*/

public class ZapMeIdentity : ClaimsIdentity
{
    private Claim CreateClaim(string type, string value, string valueType) => new Claim(type, value, valueType, ZapMeAuthenticationDefaults.AuthenticationScheme, ZapMeAuthenticationDefaults.AuthenticationScheme, this);
    private Claim CreateClaim(string type, string value) => CreateClaim(type, value, ClaimValueTypes.String);

    private IEnumerable<Claim?> CreateClaims(AccountEntity account)
    {
        //yield return CreateClaim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]);
        yield return CreateClaim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
        yield return CreateClaim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString());
        yield return CreateClaim(ClaimTypes.NameIdentifier, account.Id.ToString());
        yield return CreateClaim(ClaimTypes.Name, account.Name);
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

    public ZapMeIdentity(SessionEntity session) : base(ZapMeAuthenticationDefaults.AuthenticationScheme)
    {
        Session = session;
        AddClaims(CreateClaims(Account));
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

    public SessionEntity Session { get; }
    public AccountEntity Account => Session.Account;

    public static string SessionIdClaimType => ZapMeAuthenticationDefaults.AuthenticationScheme + ".SessionId";
    public Guid SessionId => Session.Id;

    public static string AccountIdClaimType => ClaimTypes.NameIdentifier;
    public Guid AccountId => Account.Id;

    public static string AccountNameClaimType => ClaimTypes.Name;
    public string AccountName => Account.Name;

    public IEnumerable<string> Roles => FindAll(ClaimTypes.Role).Select(claim => claim.Value);

    public static string SecurityStampClaimType => ZapMeAuthenticationDefaults.AuthenticationScheme + ".SecurityStamp";
    public Guid SecurityStamp => GetGuid(SecurityStampClaimType);
}

