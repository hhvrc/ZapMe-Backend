using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using ZapMe.Database;
using ZapMe.DTOs;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

[EnableCors]
[Consumes(Application.Json)]
[Produces(Application.Json)]
[ProducesErrorResponseType(typeof(ErrorDetails))]
[ApiController, Route("api/v1/[Controller]/")]
public sealed partial class ConfigController : ControllerBase
{
    private readonly DatabaseContext _dbContext;
    private readonly ILogger<ConfigController> _logger;

    public ConfigController(DatabaseContext dbContext, ILogger<ConfigController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
}
