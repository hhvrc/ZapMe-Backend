using Microsoft.AspNetCore.Mvc;
using ZapMe.Controllers.Api.V1.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ZapMe.Controllers.Api.V1;

public partial class ConfigController
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns>The config for the service</returns>
    /// <response code="200">Returns the service config</response>
    [HttpGet(Name = "GetConfig")]
    [Produces(Application.Json, Application.Xml)]
    [ProducesResponseType(typeof(Config.Models.Config), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDetails), StatusCodes.Status500InternalServerError)]
    public Config.Models.Config GetConfig()
    {
        return new Config.Models.Config { ProductName = "ZapMe" };
    }
}
