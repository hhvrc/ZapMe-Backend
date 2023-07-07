using Marten.Schema;

namespace ZapMe.Database.Documents;

public sealed class ControlGrantDeviceDetails
{
    public Guid DeviceId { get; init; }

    /// <summary>
    /// Max strength of a command in fractional units. 0 for unlimited.
    /// <para>Range: 0 - 65535 (0.0 - 1.0)</para>
    /// </summary>
    public ushort MaxStrength { get; set; }

    /// <summary>
    /// Max duration a command can be executed for in milliseconds. 0 for unlimited.
    /// <para>Range: 0 - 65535 (0 milliseconds - 1 minute 5 seconds)</para>
    /// </summary>
    public ushort MaxDuration { get; set; }

    /// <summary>
    /// Cooldown between command executions in milliseconds. 0 for no cooldown.
    /// <para>Range: 0 - 4294967295 (0 milliseconds - 50 days)</para>
    /// </summary>
    public uint Cooldown { get; set; }
}

public sealed class ControlGrantDeviceMap
{
    public required string Name { get; init; }

    /// <summary>
    /// Max strength of a command in fractional units. 0 for unlimited.
    /// <para>Range: 0 - 65535 (0.0 - 1.0)</para>
    /// </summary>
    public ushort MaxStrength { get; set; }

    public Guid ImageId { get; init; }

    public Dictionary<Guid, (float x, float y)> DevicePositions { get; set; } = new();
}

[DocumentAlias("control_grant_details")]
public sealed class ControlGrantDetails
{
    /// <summary>
    /// The ID of the <see cref="Models.ControlGrantEntity"/> that this document represents.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// [FOR ENTIRE GRANT] Max strength of a command in fractional units. 0 for unlimited.
    /// <para>Range: 0 - 65535 (0.0 - 1.0)</para>
    /// </summary>
    public ushort MaxStrength { get; set; }

    /// <summary>
    /// [FOR ENTIRE GRANT] Max duration a command can be executed for in milliseconds. 0 for unlimited.
    /// <para>Range: 0 - 65535 (0 milliseconds - 1 minute 5 seconds)</para>
    /// </summary>
    public ushort MaxDuration { get; set; }

    /// <summary>
    /// [FOR ENTIRE GRANT] Cooldown between command executions in milliseconds. 0 for no cooldown.
    /// <para>Range: 0 - 4294967295 (0 milliseconds - 50 days)</para>
    /// </summary>
    public uint Cooldown { get; set; }

    /// <summary>
    /// Individual devices that the grantee's have access to.
    /// </summary>
    public List<ControlGrantDeviceDetails> Devices { get; set; } = new();

    /// <summary>
    /// Device maps that the grantee's have access to.
    /// <para>A device map is a collection of devices laid out on a 2D plane as a representation of a physical or virtual space.</para>
    /// </summary>
    public List<ControlGrantDeviceMap> DeviceMaps { get; set; } = new();
}