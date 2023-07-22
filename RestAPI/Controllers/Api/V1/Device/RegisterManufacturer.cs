using Microsoft.AspNetCore.Mvc;
using ZapMe.Attributes;
using ZapMe.Database.Models;
using ZapMe.DTOs;

namespace ZapMe.Controllers.Api.V1;

public partial class DeviceController
{
    public readonly struct RegisterDeviceManufacturerRequest
    {
        [Displayname(true)]
        public required string Name { get; init; }

        public required string WebsiteUrl { get; init; }

        public Guid IconId { get; init; }
    }

    /// <summary>
    /// Register a device manufacturer.
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Device</response>
    [HttpPost("manufacturer", Name = "RegisterDeviceManufacturer")]
    [ProducesResponseType(typeof(DeviceManufacturerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RegisterManufacturer(RegisterDeviceManufacturerRequest data, CancellationToken cancellationToken)
    {
        var deviceManufacturer = new DeviceManufacturerEntity
        {
            Name = data.Name,
            WebsiteUrl = data.WebsiteUrl,
            IconId = data.IconId,
        };

        _dbContext.DeviceManufacturers.Add(deviceManufacturer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
