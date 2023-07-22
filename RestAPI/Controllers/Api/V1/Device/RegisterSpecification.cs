using Microsoft.AspNetCore.Mvc;
using ZapMe.Database.Documents;
using ZapMe.DTOs;

namespace ZapMe.Controllers.Api.V1;

public partial class DeviceController
{
    public readonly struct RegisterDeviceSpecificationRequest
    {
        public ulong Frequency { get; init; }
        public DeviceProprietarySpecification.ModulationType Modulation { get; init; }
        public ulong DataRate { get; init; }
        public ulong PacketSize { get; init; }
        public DeviceProprietarySpecification.EncodingType Encoding { get; init; }
        public bool InvertedBits { get; init; }
    }

    /// <summary>
    /// Register a device specification.
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Device</response>
    [HttpPost("specification", Name = "RegisterDeviceSpecification")]
    [ProducesResponseType(typeof(DeviceSpecificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RegisterSpecification(RegisterDeviceSpecificationRequest data, [FromServices] Marten.IDocumentStore documentStore, CancellationToken cancellationToken)
    {
        var device = new DeviceProprietarySpecification
        {
            Frequency = data.Frequency,
            Modulation = data.Modulation,
            DataRate = data.DataRate,
            PacketSize = data.PacketSize,
            Encoding = data.Encoding,
            InvertedBits = data.InvertedBits,
        };

        using var session = documentStore.LightweightSession();
        session.Store(device);
        await session.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
