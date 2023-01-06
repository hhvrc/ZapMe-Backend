using Microsoft.AspNetCore.Mvc;
using ZapMe.Authentication;
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
    [ProducesResponseType(typeof(Account.Models.AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)]
    public IActionResult Get()
    {
        ZapMeIdentity identity = (User as ZapMePrincipal)!.Identity;

        return Ok(new Account.Models.AccountDto(identity.Account)); // TODO: use a mapper FFS
    }
}