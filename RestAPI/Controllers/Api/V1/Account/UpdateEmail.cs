using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.DTOs;
using ZapMe.DTOs.API.User;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;

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
    [RequestSizeLimit(1024)]
    [HttpPut("email", Name = "UpdateAccountEmail")]
    [ProducesResponseType(typeof(UpdateEmailOk), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmail body, [FromServices] IEmailVerificationManager emailVerificationManager, CancellationToken cancellationToken)
    {
        var user = await User.VerifyUserPasswordAsync(body.Password, _dbContext, cancellationToken);
        if (user is null)
        {
            return HttpErrors.UnauthorizedActionResult;
        }

        ErrorDetails? errorDetails = await emailVerificationManager.InitiateEmailVerificationAsync(user, body.NewEmail, cancellationToken);
        if (errorDetails.HasValue)
        {
            return errorDetails.Value.ToActionResult();
        }

        return Ok(new UpdateEmailOk { Message = "Please check your email to verify your new address." });
    }
}