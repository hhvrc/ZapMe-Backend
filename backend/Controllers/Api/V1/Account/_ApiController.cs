using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Data;
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
    private readonly ZapMeContext _dbContext;
    private readonly IUserManager _userManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(ZapMeContext dbContext, IUserManager userManager, ILogger<AccountController> logger)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _logger = logger;
    }
}