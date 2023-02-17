using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

/// <summary>
/// 
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/[Controller]/")]
public sealed partial class AccountController : ControllerBase
{
    private readonly IAccountManager _accountManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAccountManager userManager, ILogger<AccountController> logger)
    {
        _accountManager = userManager;
        _logger = logger;
    }
}