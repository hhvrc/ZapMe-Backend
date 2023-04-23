using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Account.OAuth.Models;
using ZapMe.Controllers.Api.V1.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Add a oauth connection to account
    /// </summary>
    /// <returns></returns>
    [HttpPost("oauth/{providerName}", Name = "AddOAuthProvider")]
    [Consumes(Application.Json)]
    [Produces(Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status409Conflict)] // Provider already added
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)] // Provider not supported
    public IActionResult OAuthAdd([FromRoute] string providerName, [FromBody] AccountOAuthAdd body)
    {
        return Ok(String.Empty);
    }
}