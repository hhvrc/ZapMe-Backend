﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using ZapMe.Attributes;
using ZapMe.Authentication;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Utils;
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
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(
        [FromHeader][Password(true)] string password,
        [FromHeader][StringLength(1024)] string? reason,
        CancellationToken cancellationToken
        )
    {
        ZapMeIdentity identity = (User as ZapMePrincipal)!.Identity;

        if (!PasswordUtils.CheckPassword(password, identity.User.PasswordHash))
        {
            return this.Error_InvalidPassword();
        }

        await _dbContext.Users.Where(u => u.Id == identity.User.Id).ExecuteDeleteAsync(cancellationToken);

        // TODO: register reason if supplied

        return Ok();
    }
}