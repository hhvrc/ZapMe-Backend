﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Database.Models;

public sealed class FriendRequestEntity
{
    public Guid SenderId { get; set; }

    public Guid ReceiverId { get; set; }

    public DateTime CreatedAt { get; set; }

    public UserEntity Sender { get; private set; } = null!;
    public UserEntity Receiver { get; private set; } = null!;
}

public sealed class FriendRequestEntityConfiguration : IEntityTypeConfiguration<FriendRequestEntity>
{
    public void Configure(EntityTypeBuilder<FriendRequestEntity> builder)
    {
        builder.HasKey(fr => new { fr.SenderId, fr.ReceiverId });
        builder.Property(fr => fr.CreatedAt).HasDefaultValueSql("now()");

        builder.HasOne(fr => fr.Sender)
            .WithMany(u => u.FriendRequestsOutgoing)
            .HasForeignKey(fr => fr.SenderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fr => fr.Receiver)
            .WithMany(u => u.FriendRequestsIncoming)
            .HasForeignKey(fr => fr.ReceiverId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}