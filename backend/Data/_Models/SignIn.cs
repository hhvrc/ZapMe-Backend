using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Data.Models;

public sealed class SignInEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public required AccountEntity User { get; set; }

    public required string DeviceName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsValid => DateTime.UtcNow < ExpiresAt;
}

public sealed class SignInEntityConfiguration : IEntityTypeConfiguration<SignInEntity>
{
    public void Configure(EntityTypeBuilder<SignInEntity> builder)
    {
        builder.ToTable("signIns");

        builder.HasKey(si => si.Id);

        builder.Property(si => si.UserId)
            .HasColumnName("userId");

        builder.Property(si => si.DeviceName)
            .HasColumnName("deviceName")
            .HasMaxLength(32);

        builder.Property(si => si.CreatedAt)
            .HasColumnName("createdAt")
            .HasDefaultValueSql("now()");

        builder.Property(si => si.ExpiresAt)
            .HasColumnName("expiresAt");

        builder.Ignore(si => si.IsValid);

        builder.HasOne(si => si.User)
            .WithMany(u => u.SignIns)
            .HasForeignKey(si => si.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}