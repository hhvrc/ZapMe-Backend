using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Remove a oauth connection from account
    /// </summary>
    /// <returns></returns>
    [HttpDelete("oauth/{providerName}", Name = "RemoveOAuthProvider")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult OAuthRemove([FromRoute] string providerName)
    {
        return Ok();
    }
}