﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Database.Models;

public sealed class LockOutEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? Reason { get; set; }

    public required string Flags { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public UserEntity User { get; private set; } = null!;
}

public sealed class LockOutEntityConfiguration : IEntityTypeConfiguration<LockOutEntity>
{
    public void Configure(EntityTypeBuilder<LockOutEntity> builder)
    {
        builder.HasKey(lo => lo.Id);
        builder.Property(lo => lo.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.HasOne(lo => lo.User)
            .WithMany(u => u.LockOuts)
            .HasForeignKey(lo => lo.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}