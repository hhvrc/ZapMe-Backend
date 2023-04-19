using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;
using ZapMe.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Verify the users email address
    /// </summary>
    /// <param name="token"></param> 
    /// <param name="mailAddressVerificationRequestStore"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Ok</response>
    /// <response code="404">Error details</response>
    [AllowAnonymous]
    [RequestSizeLimit(1024)]
    [HttpPost("email/verify", Name = "Verify Email Address")]
    [Consumes(Application.Json, Application.Xml)]
    [Produces(Application.Json, Application.Xml)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromServices] IMailAddressVerificationRequestStore mailAddressVerificationRequestStore, CancellationToken cancellationToken)
    {
        string tokenHash = HashingUtils.Sha256_String(token);

        MailAddressChangeRequestEntity? mailAddressVerificationRequest = await mailAddressVerificationRequestStore.GetByTokenHashAsync(tokenHash, cancellationToken);
        /*if (mailAddressVerificationRequest == null)
        {
            return this.Error();
        }

        bool success = await _userManager.Store.SetEmailVerifiedAsync(mailAddressVerificationRequest.UserId, true, cancellationToken);
        */
        return Ok();
    }
}