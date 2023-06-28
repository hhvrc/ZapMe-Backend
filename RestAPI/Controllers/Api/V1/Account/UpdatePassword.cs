using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZapMe.BusinessLogic.Users;
using ZapMe.DTOs.API.User;
using ZapMe.Helpers;
using VerificationResult = ZapMe.BusinessLogic.Users.PasswordLogic.ChangePasswordWithVerificationResult;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Updates the account password
    /// </summary>
    /// <response code="200">Ok</response>
    [RequestSizeLimit(1024)]
    [HttpPut("password", Name = "AccountPasswordUpdate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update([FromBody] UpdatePassword body, CancellationToken cancellationToken)
    {
        var result = await PasswordLogic.ChangePassword_WithVerification(_dbContext, User.GetUserId(), body.CurrentPassword, body.NewPassword, cancellationToken);

        return result switch
        {
            VerificationResult.Success => NoContent(),
            VerificationResult.InvalidPassword => BadRequest(),
            VerificationResult.UserNotFound => HttpErrors.UnauthorizedActionResult,
            _ => throw new NotImplementedException(),
        };
    }
}