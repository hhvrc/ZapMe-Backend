using Microsoft.AspNetCore.Mvc;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.Account.Models;
using ZapMe.Controllers.Api.V1.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Account</response>
    /// <response code="404">Error details</response>
    [HttpGet(Name = "GetAccount")]
    [Produces(Application.Json, Application.Xml)]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status401Unauthorized)]
    public AccountDto Get()
    {
        ZapMeIdentity identity = (User as ZapMePrincipal)!.Identity;

        return new AccountDto(identity.Account); // TODO: use a mapper FFS
    }
}