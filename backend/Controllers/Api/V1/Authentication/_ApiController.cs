using Microsoft.AspNetCore.Mvc;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

/// <summary>
/// 
/// </summary>
[ApiController]
[Route("/api/v1/auth/")]
public sealed partial class AuthenticationController : ControllerBase
{
    private readonly ISessionManager _sessionManager;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(ISessionManager sessionManager, ILogger<AuthenticationController> logger)
    {
        _sessionManager = sessionManager;
        _logger = logger;
    }
}
