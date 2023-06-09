using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Constants;
using ZapMe.Controllers.Api.V1.Models;
using ZapMe.Data;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

[EnableCors]
[Consumes(Application.Json)]
[Produces(Application.Json)]
[ProducesErrorResponseType(typeof(ErrorDetails))]
[ApiController, Route("api/v1/[Controller]/")]
[Authorize(AuthSchemes.Main)]
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