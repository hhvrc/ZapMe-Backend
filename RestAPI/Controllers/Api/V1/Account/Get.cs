using Microsoft.AspNetCore.Mvc;
using ZapMe.Authentication;
using ZapMe.DTOs;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Account</response>
    [HttpGet(Name = "GetAccount")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public AccountDto Get()
    {
        ZapMeIdentity identity = (User as ZapMePrincipal)!.Identity;

        return identity.User.ToAccountDto();
    }
}