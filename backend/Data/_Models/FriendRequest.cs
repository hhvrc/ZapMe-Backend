﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Data.Models;

public sealed class FriendRequestEntity
{
    public Guid SenderId { get; set; }
    public required AccountEntity Sender { get; set; }

    public Guid ReceiverId { get; set; }
    public required AccountEntity Receiver { get; set; }

    public DateTime CreatedAt { get; set; }
}

public sealed class FriendRequestEntityConfiguration : IEntityTypeConfiguration<FriendRequestEntity>
{
    public void Configure(EntityTypeBuilder<FriendRequestEntity> builder)
    {
        builder.ToTable("friendRequests");

        builder.HasKey(fr => new { fr.SenderId, fr.ReceiverId });

        builder.Property(fr => fr.SenderId)
            .HasColumnName("senderId");

        builder.Property(fr => fr.ReceiverId)
            .HasColumnName("receiverId");

        builder.Property(fr => fr.CreatedAt)
            .HasColumnName("createdAt")
            .HasDefaultValueSql("now()");

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