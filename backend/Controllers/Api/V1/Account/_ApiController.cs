using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

/// <summary>
/// 
/// </summary>
[Authorize]
[EnableCors]
[ApiController]
[Route("api/v1/[Controller]/")]
public sealed partial class AccountController : ControllerBase
{
    private readonly IUserManager _userManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IUserManager userManager, ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }
}