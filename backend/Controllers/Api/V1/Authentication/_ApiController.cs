using Microsoft.AspNetCore.Mvc;
using ZapMe.Data;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/api/v1/auth/")]
public sealed partial class AuthenticationController : ControllerBase
{
    private readonly ZapMeContext _dbContext;
    private readonly ISessionManager _sessionManager;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(ZapMeContext dbContext, ISessionManager sessionManager, ILogger<AuthenticationController> logger)
    {
        _dbContext = dbContext;
        _sessionManager = sessionManager;
        _logger = logger;
    }
}
