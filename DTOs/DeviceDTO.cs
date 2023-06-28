namespace ZapMe.DTOs;

public readonly struct DeviceDto
{
    public Guid Id { get; init; }

    public DateTime RegisteredAt { get; init; }
}