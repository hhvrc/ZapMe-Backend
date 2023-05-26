using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Models;

namespace ZapMe.Controllers.Api.V1;

/// <summary>
/// 
/// </summary>
[ProducesErrorResponseType(typeof(ErrorDetails))]
[ApiController, Route("api/v1/[Controller]/")]
public sealed partial class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }
}

