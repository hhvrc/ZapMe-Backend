using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Helpers;

namespace ZapMe.Controllers.Api.V1;

public partial class DeviceController
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Device</response>
    [HttpGet("{deviceId}", Name = "GetDeviceById")]
    [ProducesResponseType(typeof(DeviceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get(Guid deviceId, CancellationToken cancellationToken)
    {
        DeviceEntity? device = await _dbContext
            .Devices
            .AsNoTracking()
            .Include(d => d.Icon)
            .Include(d => d.Model).ThenInclude(m => m!.Icon)
            .Include(d => d.Model).ThenInclude(m => m!.Manufacturer)
            .Include(d => d.Model).ThenInclude(m => m!.Manufacturer).ThenInclude(m => m!.Icon)
            .Include(d => d.Owner)
            .FirstOrDefaultAsync(d => d.Id == deviceId, cancellationToken);
        if (device is null)
        {
            return HttpErrors.DeviceNotFoundActionResult;
        }

        return Ok(DeviceMapper.MapToDeviceDto(device));
    }
}
