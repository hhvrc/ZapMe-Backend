using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Data.Models;

public sealed class SessionEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public required AccountEntity User { get; set; }

    public required string Name { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}

public sealed class SessionConfiguration : IEntityTypeConfiguration<SessionEntity>
{
    public void Configure(EntityTypeBuilder<SessionEntity> builder)
    {
        builder.ToTable("sessions");

        builder.HasKey(si => si.Id);

        builder.Property(si => si.UserId)
            .HasColumnName("userId");

        builder.Property(si => si.Name)
            .HasColumnName("name")
            .HasMaxLength(32);

        builder.Property(si => si.CreatedAt)
            .HasColumnName("createdAt")
            .HasDefaultValueSql("now()");

        builder.Property(si => si.ExpiresAt)
            .HasColumnName("expiresAt");

        builder.Ignore(si => si.IsExpired);

        builder.HasOne(si => si.User)
            .WithMany(u => u.Sessions)
            .HasForeignKey(si => si.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}