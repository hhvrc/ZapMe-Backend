using Microsoft.AspNetCore.Mvc;

namespace ZapMe.Controllers.Api.V1;

/// <summary>
/// 
/// </summary>
[ApiController, Route("api/v1/[Controller]/")]
public sealed partial class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }
}

