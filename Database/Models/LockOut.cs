using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Database.Models;

public sealed class LockOutEntity
{
    public Guid Id { get; private set; }

    public Guid UserId { get; init; }

    public string? Reason { get; set; }

    public required string Flags { get; set; }

    public DateTime CreatedAt { get; init; }

    public DateTime? ExpiresAt { get; set; }

    public UserEntity User { get; private set; } = null!;
}

public sealed class LockOutEntityConfiguration : IEntityTypeConfiguration<LockOutEntity>
{
    public void Configure(EntityTypeBuilder<LockOutEntity> builder)
    {
        builder.Property(lo => lo.Id).HasDefaultValueSql("gen_random_uuid()");
    }
}