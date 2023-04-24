using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Verify the users email address
    /// </summary>
    /// <param name="token"></param> 
    /// <param name="emailVerificationManager"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Ok</response>
    /// <response code="404">Error details</response>
    [AllowAnonymous]
    [RequestSizeLimit(1024)]
    [HttpPost("email/verify", Name = "Verify Email Address")]
    [Consumes(Application.Json)]
    [Produces(Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)] // Token invalid, expired, or already used
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromServices] IEmailVerificationManager emailVerificationManager, CancellationToken cancellationToken)
    {
        ErrorDetails? errorDetails = await emailVerificationManager.CompleteEmailVerificationAsync(token, cancellationToken);
        if (errorDetails.HasValue)
        {
            return errorDetails.Value.ToActionResult();
        }

        return Ok();
    }
}