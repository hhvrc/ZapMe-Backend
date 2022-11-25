using Microsoft.AspNetCore.Mvc;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.DTOs;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Delete currently logged in account
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Empty</response>
    /// <response code="400"></response>
    /// <response code="500"></response>
    [RequestSizeLimit(1024)]
    [HttpDelete(Name = "DeleteAccount")]
    [Produces(Application.Json, Application.Xml)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromHeader] string password, [FromQuery] string? reason, CancellationToken cancellationToken)
    {
        ZapMeIdentity identity = (User.Identity as ZapMeIdentity)!;

        PasswordCheckResult result = _userManager.CheckPassword(session.User, password, cancellationToken);
        switch (result)
        {
            case PasswordCheckResult.Success:
                break;
            case PasswordCheckResult.PasswordInvalid:
                return this.Error_InvalidPassword();
            case PasswordCheckResult.UserNotFound:
            default:
                return this.Error_InternalServerError();
        }

        await _userManager.DeleteAsync(session.UserId, cancellationToken);

        // TODO: register reason if supplied

        return Ok();
    }
}