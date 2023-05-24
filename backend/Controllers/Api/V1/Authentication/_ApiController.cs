using Microsoft.AspNetCore.Mvc;
using ZapMe.Data;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

/// <summary>
/// 
/// </summary>
[ApiController, Route("api/v1/[Controller]/")]
public sealed partial class AuthController : ControllerBase
{
    private readonly ZapMeContext _dbContext;
    private readonly ISessionManager _sessionManager;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ZapMeContext dbContext, ISessionManager sessionManager, ILogger<AuthController> logger)
    {
        _dbContext = dbContext;
        _sessionManager = sessionManager;
        _logger = logger;
    }
}
