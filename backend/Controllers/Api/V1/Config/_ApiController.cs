using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ZapMe.Controllers.Api.V1;

/// <summary>
/// 
/// </summary>
[EnableCors]
[ApiController]
[Route("/api/v1/config/")]
public sealed partial class ConfigController : Controller
{
    private readonly ILogger<ConfigController> _logger;

    public ConfigController(ILogger<ConfigController> logger)
    {
        _logger = logger;
    }
}
