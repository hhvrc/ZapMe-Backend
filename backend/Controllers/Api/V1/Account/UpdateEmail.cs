using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.Account.Models;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data;
using ZapMe.Helpers;
using ZapMe.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class AccountController
{
    /// <summary>
    /// Updates the account email
    /// </summary>
    /// <param name="body"></param>
    /// <param name="dbContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">New email</response>
    /// <response code="400">Error details</response>
    [RequestSizeLimit(1024)]
    [HttpPut("email", Name = "UpdateEmail")]
    [Consumes(Application.Json)]
    [Produces(Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmail body, [FromServices] ZapMeContext dbContext, CancellationToken cancellationToken)
    {
        ZapMeIdentity identity = (User.Identity as ZapMeIdentity)!;

        if (!PasswordUtils.CheckPassword(body.Password, identity.User.PasswordHash))
        {
            return CreateHttpError.InvalidPassword().ToActionResult();
        }

        await dbContext.Users.Where(u => u.Id == identity.UserId).ExecuteUpdateAsync(spc => spc.SetProperty(u => u.Email, _ => null), cancellationToken);

        return Ok();
    }
}