using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Data;
using ZapMe.Services.Interfaces;

namespace ZapMe.Controllers.Api.V1;

/// <summary>
/// 
/// </summary>
[ApiController, Authorize, Route("api/v1/[Controller]/")]
public sealed partial class UserController : ControllerBase
{
    private readonly ZapMeContext _dbContext;
    private readonly IUserStore _userStore;

    public UserController(ZapMeContext dbContext, IUserStore userManager)
    {
        _dbContext = dbContext;
        _userStore = userManager;
    }
}