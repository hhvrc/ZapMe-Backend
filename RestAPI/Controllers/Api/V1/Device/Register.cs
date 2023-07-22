using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ZapMe.Attributes;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Utils;

namespace ZapMe.Controllers.Api.V1;

public partial class DeviceController
{
    public readonly record struct RegisterDeviceRequest
    {
        public Guid DeviceModelId { get; init; }

        [Displayname(true)]
        public string Name { get; init; }
    }

    /// <summary>
    /// Register a device.
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Device</response>
    [HttpPost(Name = "RegisterDevice")]
    [ProducesResponseType(typeof(DeviceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Register(RegisterDeviceRequest data, CancellationToken cancellationToken)
    {
        var deviceModel = await _dbContext.DeviceModels
            .AsNoTracking()
            .Where(dm => dm.Id == data.DeviceModelId)
            .FirstOrDefaultAsync(cancellationToken);

        if (deviceModel is null)
            return NotFound();

        var device = new DeviceEntity
        {
            ModelId = deviceModel.Id,
            OwnerId = User.GetUserId(),
            Name = data.Name,
            AccessToken = StringUtils.GenerateUrlSafeRandomString(64)
        };

        _dbContext.Devices.Add(device);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
