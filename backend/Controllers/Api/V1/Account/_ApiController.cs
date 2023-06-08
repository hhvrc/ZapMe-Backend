using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Constants;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

/// <summary>
/// 
/// </summary>
[Consumes(Application.Json)]
[Produces(Application.Json)]
[ProducesErrorResponseType(typeof(ErrorDetails))]
[ApiController, Route("api/v1/[Controller]/")]
[Authorize(AuthSchemes.Main)]
[ResponseCache(CacheProfileName = "no-store")]
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