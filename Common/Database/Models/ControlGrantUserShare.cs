using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Database.Models;

public sealed class ControlGrantUserShareEntity
{
    public Guid GrantId { get; private set; }

    public Guid GrantedUserId { get; init; }

    public uint? UsesLeft { get; set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? ExpiresAt { get; set; }

    public ControlGrantEntity? Grant { get; private set; }

    public UserEntity? GrantedUser { get; private set; }
}

public sealed class ControlGrantUserShareEntityConfiguration : IEntityTypeConfiguration<ControlGrantUserShareEntity>
{
    public void Configure(EntityTypeBuilder<ControlGrantUserShareEntity> builder)
    {
        builder.HasKey(ud => new { ud.GrantId, ud.GrantedUserId });
        builder.Property(ud => ud.CreatedAt).HasDefaultValueSql("now()");
    }
}