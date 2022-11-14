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
    private readonly ISignInManager _signInManager;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(ISignInManager signInManager, ILogger<AuthenticationController> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }
}
