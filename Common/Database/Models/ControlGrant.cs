using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Database.Models;

public sealed class ControlGrantEntity
{
    public Guid Id { get; private set; }

    public Guid SubjectUserId { get; init; }

    public required string Name { get; init; }

    public UserEntity? SubjectUser { get; private set; }

    public List<ControlGrantUserShareEntity> UserShares { get; private set; } = new();

    public List<ControlGrantPublicShareEntity> PublicShares { get; private set; } = new();
}

public sealed class ControlGrantEntityConfiguration : IEntityTypeConfiguration<ControlGrantEntity>
{
    public void Configure(EntityTypeBuilder<ControlGrantEntity> builder)
    {
        builder.HasKey(cg => cg.Id);

        builder.HasMany(cg => cg.UserShares)
            .WithOne(cgus => cgus.Grant)
            .HasForeignKey(cgus => cgus.GrantId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(cg => cg.PublicShares)
            .WithOne(cgps => cgps.Grant)
            .HasForeignKey(cgps => cgps.GrantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}