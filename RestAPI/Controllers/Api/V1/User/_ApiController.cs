﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZapMe.DTOs;
using ZapMe.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

[EnableCors]
[Consumes(Application.Json)]
[Produces(Application.Json)]
[ProducesErrorResponseType(typeof(ErrorDetails))]
[ApiController, Authorize, Route("api/v1/[Controller]/")]
public sealed partial class UserController : ControllerBase
{
    private readonly IUserRelationManager _userManager;

    public UserController(IUserRelationManager userManager)
    {
        _userManager = userManager;
    }
}