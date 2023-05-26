using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Models;

namespace ZapMe.Controllers.Api.V1;

/// <summary>
/// 
/// </summary>
[ProducesErrorResponseType(typeof(ErrorDetails))]
[ApiController, Route("api/v1/[Controller]/")]
public sealed partial class ConfigController : ControllerBase
{
    private readonly ILogger<ConfigController> _logger;

    public ConfigController(ILogger<ConfigController> logger)
    {
        _logger = logger;
    }
}
