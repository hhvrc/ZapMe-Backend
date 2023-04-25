using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Controllers.Api.V1.User.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class UserController
{
    /// <summary>
    /// Report a user
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    /// <response code="404"></response>
    [HttpPost("report", Name = "ReportUser")]
    [Consumes(Application.Json)]
    [Produces(Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)] // Report sent
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status404NotFound)] // User not found
    public IActionResult Report([FromBody] UserReport body)
    {
        Console.WriteLine($"ReportUser: {body.UserId}");
        Console.WriteLine($"Reason: {body.Title}");

        return Ok();
    }
}