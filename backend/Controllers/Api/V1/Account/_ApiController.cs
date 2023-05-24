using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Data;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

/// <summary>
/// 
/// </summary>
[ApiController, Authorize, Route("api/v1/[Controller]/")]
public sealed partial class AccountController : ControllerBase
{
    private readonly ZapMeContext _dbContext;
    private readonly IUserStore _userStore;
    private readonly ILogger<AccountController> _logger;

    public AccountController(ZapMeContext dbContext, IUserStore userManager, ILogger<AccountController> logger)
    {
        _dbContext = dbContext;
        _userStore = userManager;
        _logger = logger;
    }
}