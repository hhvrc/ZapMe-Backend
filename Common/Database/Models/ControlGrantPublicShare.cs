using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Database.Models;

public sealed class ControlGrantPublicShareEntity
{
    public Guid GrantId { get; private set; }

    public required string AccessKey { get; init; }

    public uint? UsesLeft { get; set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? ExpiresAt { get; set; }

    public ControlGrantEntity? Grant { get; private set; }
}

public sealed class ControlGrantPublicShareEntityConfiguration : IEntityTypeConfiguration<ControlGrantPublicShareEntity>
{
    public void Configure(EntityTypeBuilder<ControlGrantPublicShareEntity> builder)
    {
        builder.HasKey(ud => new { ud.GrantId, ud.AccessKey });
        builder.Property(ud => ud.CreatedAt).HasDefaultValueSql("now()");

        builder.HasIndex(ud => ud.GrantId);
        builder.HasIndex(ud => ud.AccessKey).IsUnique();
    }
}