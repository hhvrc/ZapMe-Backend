using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ZapMe.Database;
using ZapMe.DTOs;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

[EnableCors]
[Consumes(Application.Json)]
[Produces(Application.Json)]
[ProducesErrorResponseType(typeof(ErrorDetails))]
[ApiController, Authorize, Route("api/v1/[Controller]/")]
public sealed partial class UserController : ControllerBase
{
    private readonly IDistributedCache _cache;
    private readonly DatabaseContext _dbContext;
    private readonly ILogger<UserController> _logger;

    public UserController(IDistributedCache cache, DatabaseContext dbContext, ILogger<UserController> logger)
    {
        _cache = cache;
        _dbContext = dbContext;
        _logger = logger;
    }
}