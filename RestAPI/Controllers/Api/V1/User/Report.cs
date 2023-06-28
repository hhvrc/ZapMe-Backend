using Microsoft.AspNetCore.Mvc;
using ZapMe.DTOs.Moderation;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Report a user
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    [HttpPost("report", Name = "CreateUserReport")]
    [ProducesResponseType(StatusCodes.Status200OK)] // Report sent
    [ProducesResponseType(StatusCodes.Status404NotFound)] // User not found
    public IActionResult Report([FromBody] UserReport body)
    {
        Console.WriteLine($"ReportUser: {body.UserId}");
        Console.WriteLine($"Reason: {body.Title}");

        return Ok();
    }
}