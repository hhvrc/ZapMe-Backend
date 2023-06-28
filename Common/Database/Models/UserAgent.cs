using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;

namespace ZapMe.Database.Models;

public sealed class UserAgentEntity
{
    public Guid Id { get; private set; }

    public required string Sha256 { get; init; }

    public uint Length { get; init; }

    public required string Value { get; init; }

    public required string OperatingSystem { get; init; }

    public required string Device { get; init; }

    public required string Browser { get; init; }

    public DateTime CreatedAt { get; init; }

    public List<SessionEntity> Sessions { get; private set; } = new List<SessionEntity>();
}

public sealed class UserAgentEntityConfiguration : IEntityTypeConfiguration<UserAgentEntity>
{
    public void Configure(EntityTypeBuilder<UserAgentEntity> builder)
    {
        builder.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(u => u.Sha256).HasMaxLength(HashConstants.Sha256LengthHex);
        builder.Property(u => u.Value).HasMaxLength(UserAgentLimits.StoredValueLength);
        builder.Property(u => u.OperatingSystem).HasMaxLength(UserAgentLimits.StoredOperatingSystemLength);
        builder.Property(u => u.Device).HasMaxLength(UserAgentLimits.StoredDeviceLength);
        builder.Property(u => u.Browser).HasMaxLength(UserAgentLimits.StoredBrowserLength);
        builder.Property(u => u.CreatedAt).HasDefaultValueSql("now()");
        builder.HasIndex(u => u.Sha256).IsUnique();
    }
}