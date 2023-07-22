using ZapMe.Enums.Devices;

namespace ZapMe.DTOs;

public readonly struct DeviceModelDto
{
    public Guid Id { get; init; }

    public string Name { get; init; }

    public Guid ModelId { get; init; }

    public string ModelName { get; init; }

    public string ModelNumber { get; init; }

    public string ModelWebsiteUrl { get; init; }

    public Uri IconUrl { get; init; }

    public RfProtocol Protocol { get; init; }

    public Guid ManufacturerId { get; init; }

    public string ManufacturerName { get; init; }

    public string ManufacturerWebsiteUrl { get; init; }

    public DateTime RegisteredAt { get; init; }
}