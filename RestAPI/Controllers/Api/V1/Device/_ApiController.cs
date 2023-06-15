using Microsoft.AspNetCore.Authorization;
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
public sealed partial class DeviceController : ControllerBase
{
    private readonly DatabaseContext _dbContext;
    private readonly ILogger<DeviceController> _logger;

    public DeviceController(DatabaseContext dbContext, ILogger<DeviceController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
}