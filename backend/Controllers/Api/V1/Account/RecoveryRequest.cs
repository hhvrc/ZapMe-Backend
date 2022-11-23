using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Data.Models;
using ZapMe.Helpers;
using ZapMe.Services.Interfaces;
using ZapMe.Views;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Request password recovery of a account, a recovery email will be sent to the user that makes a call to the /recovery-confirm endpoint
    /// </summary>
    /// <param name="body"></param>
    /// <param name="mailServiceProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Account</response>
    [AllowAnonymous]
    [RequestSizeLimit(1024)]
    [HttpPost("recover", Name = "AccountRecoveryRequest")]
    [Consumes(Application.Json, Application.Xml)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RecoveryRequest([FromBody] Account.Models.RecoveryRequest body, [FromServices] IMailGunService mailServiceProvider, CancellationToken cancellationToken)
    {
        await using TimeLock tl = TimeLock.FromSeconds(1, cancellationToken);

        AccountEntity? account = await _userManager.GetByEmailAsync(body.Email, cancellationToken);

        if (account != null)
        {
            // Create/Overwrite recovery secret
            string? passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(account.Id, cancellationToken);
            if (passwordResetToken != null)
            {
                string render = ResetPassword.Build(account.UserName, Constants.BackendBaseUrl + "/reset-password?token=" + passwordResetToken);

                // Send recovery secret to email
                await mailServiceProvider.SendMailAsync("Hello", "hello", $"{account.UserName} <{account.Email}>", "Password recovery", render, cancellationToken);
            }
        }

        return Ok();
    }
}