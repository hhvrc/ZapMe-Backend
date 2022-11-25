using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Look up user by name
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    /// <response code="404"></response>
    [RequestSizeLimit(1024)]
    [HttpGet("u/{userName}", Name = "LookUpUser")]
    [Produces(Application.Json, Application.Xml)]
    [ProducesResponseType(typeof(User.Models.UserDto), StatusCodes.Status200OK)]     // Accepted
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)] // User not found
    public async Task<IActionResult> LookUp([FromRoute] string userName, CancellationToken cancellationToken)
    {
        var account = await _userManager.GetByUsernameAsync(userName, cancellationToken);

        if (account != null)
        {
            return Ok(account);
        }

        // Give up
        return NotFound();
    }
}