using ZapMe.Attributes;
using ZapMe.Enums;

namespace ZapMe.DTOs;

public readonly struct DeviceDto
{
    public Guid Id { get; init; }

    public DateTime RegisteredAt { get; init; }
}