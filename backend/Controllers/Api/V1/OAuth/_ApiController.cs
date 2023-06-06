using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

[Produces(Application.Json)]
[ProducesErrorResponseType(typeof(ErrorDetails))]
[ApiController, Route("api/v1/[Controller]/")]
public sealed partial class OAuthController : ControllerBase
{
    private readonly ZapMeContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public OAuthController(ZapMeContext dbContext, IConfiguration configuration, ILogger<AuthController> logger)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
    }
}
