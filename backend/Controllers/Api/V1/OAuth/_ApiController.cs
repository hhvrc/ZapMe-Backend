using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

[Produces(Application.Json)]
[ProducesErrorResponseType(typeof(ErrorDetails))]
[ApiController, Route("api/v1/[Controller]/")]
[ResponseCache(CacheProfileName = "no-store")]
public sealed partial class OAuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    public OAuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }
}
