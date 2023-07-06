using Marten.Schema;

namespace ZapMe.Database.Documents;

// Carrier wave modulation schemes
public enum RfRawModulation
{
    OOK,   // On-Off Keying
    ASK,   // Amplitude Shift Keying
    FSK,   // Frequency Shift Keying
    GFSK,  // Gaussian Frequency Shift Keying
    PSK,   // Phase Shift Keying
    QPSK,  // Quadrature Phase Shift Keying
    BPSK,  // Binary Phase Shift Keying
    QAM,   // Quadrature Amplitude Modulation
    DSSS,  // Direct Sequence Spread Spectrum
    MSK,   // Minimum Shift Keying
    OFDM,  // Orthogonal Frequency Division Multiplexing
    DCPM,  // Differential Continuous Phase Modulation
    ADPCM, // Adaptive Differential Pulse Code Modulation
    PCM,   // Pulse Code Modulation
}

// Bit-level encoding schemes
public enum RfRawEncoding
{
    None,
    Proprietary,

    Manchester,
    ManchesterInverted, // aka IEEE 802.3
    ManchesterDifferential, // aka Bi-Phase Mark, Bi-Phase Manchester, F2F, Aiken Bi-Phase, Conditioned Diphase

    BipolarRz,
    BipolarNrz,
    UnipolarRz,
    UnipolarNrz,

    // Non Return to Zero
    Rz,
    Nrz,
    NrzL, // Level
    NrzI, // Inverted
    NrzM, // Mark
    NrzS, // Space
    NrzC, // Change

    // Bi-Phase
    BiPhaseL, // Level
    BiPhaseI, // Inverted
    BiPhaseS, // Space

    // Bi-Polar
    Bipolar,
    BipolarPseudoternary,

    // Uni-Polar
    UniPolar,
    UniPolarRz,

    // Alternate Mark Inversion
    Ami,
    AmiB8ZS, // Bipolar with 8-Zero Substitution

    Block4B5B,
    Block8B6T,
}

public enum RfMessagePartRole
{
    Padding,
    Preamble,
    SyncWord,
    PayloadDeviceId,
    PayloadParameter,
    PayloadChannel,
    PayloadChecksum,
}

public sealed class MessagePart
{
    public RfMessagePartRole Role { get; init; }

    public string? ParameterName { get; init; }

    public uint LengthBits { get; init; }

    public string? ValueHex { get; init; }
}

public sealed class RfRawSpec
{
    public ulong Frequency { get; init; }

    public RfRawModulation Modulation { get; init; }

    public ulong DataRate { get; init; }

    public ulong PacketSize { get; init; }

    public RfRawEncoding Encoding { get; init; }

    public required MessagePart[] MessageSpec { get; init; }
}

public sealed class RfBleSpec
{

}

public sealed class RfWiFiSpec
{

}

[DocumentAlias("device_spec")]
public sealed class DeviceSpecification
{
    public Guid Id { get; init; }

    public RfRawSpec? RawSpec { get; init; }

    public RfBleSpec? BleSpec { get; init; }   // Future, to be implemented

    public RfWiFiSpec? WiFiSpec { get; init; } // Future, to be implemented
}
