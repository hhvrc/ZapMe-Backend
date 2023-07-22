using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZapMe.Attributes;
using ZapMe.Database.Documents;
using ZapMe.Database.Models;
using ZapMe.DTOs;
using ZapMe.Enums.Devices;

namespace ZapMe.Controllers.Api.V1;

public partial class DeviceController
{
    public readonly struct RegisterDeviceModelRequest
    {
        [Displayname(true)]
        public string Name { get; init; }

        public string ModelNumber { get; init; }

        public string WebsiteUrl { get; init; }

        public Guid IconId { get; init; }

        public Guid? ManufacturerId { get; init; }

        public string? FccId { get; init; }

        public string DocumentationUrl { get; init; }

        public RfProtocol Protocol { get; init; }

        public Guid? SpecificationId { get; init; }
    }

    /// <summary>
    /// Register a device.
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Device</response>
    [HttpPost("model", Name = "RegisterDeviceModel")]
    [ProducesResponseType(typeof(DeviceModelDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RegisterModel(RegisterDeviceModelRequest data, [FromServices] Marten.IQuerySession querySession, CancellationToken cancellationToken)
    {
        DeviceManufacturerEntity? manufacturer = null;
        if (data.ManufacturerId.HasValue)
        {
            manufacturer = await _dbContext.DeviceManufacturers
                .AsNoTracking()
                .Where(dm => dm.Id == data.ManufacturerId.Value)
                .FirstOrDefaultAsync(cancellationToken);

            if (manufacturer is null)
                return NotFound();
        }

        DeviceProprietarySpecification? specification = null;
        if (data.SpecificationId.HasValue)
        {
            specification = await querySession.LoadAsync<DeviceProprietarySpecification>(data.SpecificationId.Value, cancellationToken);

            if (specification is null)
                return NotFound();
        }

        var deviceModel = new DeviceModelEntity
        {
            Name = data.Name,
            ModelNumber = data.ModelNumber,
            WebsiteUrl = data.WebsiteUrl,
            IconId = data.IconId,
            ManufacturerId = manufacturer?.Id,
            FccId = data.FccId,
            DocumentationUrl = data.DocumentationUrl,
            Protocol = data.Protocol,
            SpecificationId = specification?.Id
        };

        _dbContext.DeviceModels.Add(deviceModel);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
