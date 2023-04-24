using Microsoft.AspNetCore.Mvc;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.Account.Models;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Updates the account email
    /// </summary>
    /// <param name="body"></param>
    /// <param name="emailVerificationManager"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Ok</response>
    /// <response code="400">Error details</response>
    [RequestSizeLimit(1024)]
    [HttpPut("email", Name = "UpdateEmail")]
    [Consumes(Application.Json)]
    [Produces(Application.Json)]
    [ProducesResponseType(typeof(UpdateEmailOk), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmail body, [FromServices] IEmailVerificationManager emailVerificationManager, CancellationToken cancellationToken)
    {
        ZapMeIdentity identity = (User.Identity as ZapMeIdentity)!;

        if (!PasswordUtils.CheckPassword(body.Password, identity.User.PasswordHash))
        {
            return CreateHttpError.InvalidPassword().ToActionResult();
        }

        ErrorDetails? errorDetails = await emailVerificationManager.InitiateEmailVerificationAsync(identity.User, body.NewEmail, cancellationToken);
        if (errorDetails.HasValue)
        {
            return errorDetails.Value.ToActionResult();
        }

        return Ok(new UpdateEmailOk { Message = "Please check your email to verify your new address." });
    }
}