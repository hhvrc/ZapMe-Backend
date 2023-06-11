﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZapMe.Database;
using ZapMe.DTOs;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

[EnableCors]
[Consumes(Application.Json)]
[Produces(Application.Json)]
[ProducesErrorResponseType(typeof(ErrorDetails))]
[ApiController, Authorize, Route("api/v1/[Controller]/")]
[ResponseCache(CacheProfileName = "no-store")]
public sealed partial class AccountController : ControllerBase
{
    private readonly DatabaseContext _dbContext;
    private readonly IUserStore _userStore;
    private readonly ILogger<AccountController> _logger;

    public AccountController(DatabaseContext dbContext, IUserStore userManager, ILogger<AccountController> logger)
    {
        _dbContext = dbContext;
        _userStore = userManager;
        _logger = logger;
    }
}