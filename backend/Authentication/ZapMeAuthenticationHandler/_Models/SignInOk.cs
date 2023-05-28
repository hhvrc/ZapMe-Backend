using ZapMe.Controllers.Api.V1.Account.Models;
using ZapMe.Data.Models;

namespace ZapMe.Authentication.Models;

/// <summary>
/// 
/// </summary>
public sealed class SignInOk
{
    public SignInOk(SessionEntity session)
    {
        ArgumentNullException.ThrowIfNull(session.User);

        Session = new SessionDto(session);
        Account = new AccountDto(session.User);
    }

    /// <summary>
    /// 
    /// </summary>
    public SessionDto Session { get; }

    /// <summary>
    /// 
    /// </summary>
    public AccountDto Account { get; }
}
