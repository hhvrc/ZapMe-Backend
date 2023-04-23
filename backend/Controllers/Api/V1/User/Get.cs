﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data.Models;
using ZapMe.Helpers;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Get user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    /// <response code="404"></response>
    [RequestSizeLimit(1024)]
    [HttpGet("i/{userId}", Name = "GetUser")]
    [Produces(Application.Json)]
    [ProducesResponseType(typeof(User.Models.UserDto), StatusCodes.Status200OK)]     // Accepted
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)] // User not found
    public async Task<IActionResult> Get([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        UserEntity? user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            return CreateHttpError.Generic(StatusCodes.Status404NotFound, "Not found", $"User with id {userId} not found").ToActionResult();
        }

        return Ok(user);
    }
}