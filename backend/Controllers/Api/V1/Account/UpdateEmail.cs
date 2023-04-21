using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data;
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
    [Consumes(Application.Json, Application.Xml)]
    [Produces(Application.Json, Application.Xml)]
    [ProducesResponseType(typeof(Account.Models.AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateEmail([FromBody] Account.Models.UpdateEmail body, [FromServices] ZapMeContext dbContext, CancellationToken cancellationToken)
    {
        ZapMeIdentity identity = (User.Identity as ZapMeIdentity)!;

        if (!PasswordUtils.CheckPassword(body.Password, identity.User.PasswordHash))
        {
            return this.Error_InvalidPassword();
        }

        await dbContext.Users.Where(u => u.Id == identity.UserId).ExecuteUpdateAsync(spc => spc.SetProperty(u => u.Email, _ => null), cancellationToken);

        return Ok();
    }
}