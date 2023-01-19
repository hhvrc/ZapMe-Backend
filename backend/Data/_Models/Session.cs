using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;

namespace ZapMe.Data.Models;

public sealed class SessionEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public required AccountEntity Account { get; set; }

    /// <summary>
    /// User provided name for this session
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The visitor's IP address (IPv4 or IPv6)
    /// This is used to prevent session hijacking
    /// The source of this value is fetched from the cloudflare provided headers, forwarded for headers, or the remote ip address
    /// </summary>
    public required string IpAddress { get; set; }

    /// <summary>
    /// The country code of the visitor's IP address (in ISO 3166-1 Alpha 2 format)
    /// This is used to prevent session hijacking
    /// If unknown, this value will be set to ZZ
    /// </summary>
    public required string CountryCode { get; set; }

    public required byte[] UserAgentHash { get; set; }
    public required UserAgentEntity UserAgent { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}

public sealed class SessionEntityConfiguration : IEntityTypeConfiguration<SessionEntity>
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

        builder.Property(si => si.IpAddress)
            .HasColumnName("ipAddress")
            .HasMaxLength(GeneralHardLimits.IPAddressMaxLength);

        builder.Property(si => si.CountryCode)
            .HasColumnName("country")
            .HasMaxLength(2);

        builder.Property(si => si.UserAgentHash)
            .HasColumnName("userAgent")
            .HasMaxLength(HashConstants.Sha256LengthBin);

        builder.Property(si => si.CreatedAt)
            .HasColumnName("createdAt")
            .HasDefaultValueSql("now()");

        builder.Property(si => si.ExpiresAt)
            .HasColumnName("expiresAt");

        builder.Ignore(si => si.IsExpired);

        builder.HasOne(si => si.Account)
            .WithMany(u => u.Sessions)
            .HasForeignKey(si => si.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(si => si.UserAgent)
            .WithMany()
            .HasForeignKey(si => si.UserAgentHash);
    }
}