using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ZapMe.Controllers.Api.V1;

partial class HealthController
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Health status</response>
    [AllowAnonymous]
    [HttpGet(Name = "HealthGet")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok("{\"msg\": \"Hello World!\"}");
    }
}

