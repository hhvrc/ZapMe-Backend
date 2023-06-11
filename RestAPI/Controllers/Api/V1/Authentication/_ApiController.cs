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
[ApiController, Route("api/v1/[Controller]/")]
[ResponseCache(CacheProfileName = "no-store")]
public sealed partial class AuthController : ControllerBase
{
    private readonly DatabaseContext _dbContext;
    private readonly ISessionManager _sessionManager;
    private readonly ILogger<AuthController> _logger;

    public AuthController(DatabaseContext dbContext, ISessionManager sessionManager, ILogger<AuthController> logger)
    {
        _dbContext = dbContext;
        _sessionManager = sessionManager;
        _logger = logger;
    }
}
