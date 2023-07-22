using ZapMe.Enums.Devices;

namespace ZapMe.DTOs;

public readonly struct DeviceManufacturerDto
{
    public Guid Id { get; init; }

    public string Name { get; init; }

    public string ModelWebsiteUrl { get; init; }

    public Uri IconUrl { get; init; }
}