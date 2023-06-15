using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;

namespace ZapMe.Database.Models;

public sealed class SessionEntity
{
    public const string TableName = "sessions";

    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    /// <summary>
    /// User provided name for this session
    /// </summary>
    public string? NickName { get; set; }

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

    public Guid UserAgentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public UserEntity User { get; private set; } = null!;
    public UserAgentEntity UserAgent { get; private set; } = null!;
}

public sealed class SessionEntityConfiguration : IEntityTypeConfiguration<SessionEntity>
{
    public void Configure(EntityTypeBuilder<SessionEntity> builder)
    {
        builder.ToTable(SessionEntity.TableName);

        builder.HasKey(si => si.Id);

        builder.Property(si => si.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(si => si.UserId)
            .HasColumnName("userId");

        builder.Property(si => si.NickName)
            .HasColumnName("nickName")
            .HasMaxLength(32);

        builder.Property(si => si.IpAddress)
            .HasColumnName("ipAddress")
            .HasMaxLength(GeneralHardLimits.IPAddressMaxLength);

        builder.Property(si => si.CountryCode)
            .HasColumnName("country")
            .HasMaxLength(2);

        builder.Property(si => si.UserAgentId)
            .HasColumnName("userAgentId");

        builder.Property(si => si.CreatedAt)
            .HasColumnName("createdAt")
            .HasDefaultValueSql("now()");

        builder.Property(si => si.ExpiresAt)
            .HasColumnName("expiresAt");

        builder.HasOne(si => si.User)
            .WithMany(u => u.Sessions)
            .HasForeignKey(si => si.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(si => si.UserAgent)
            .WithMany(ua => ua.Sessions)
            .HasForeignKey(si => si.UserAgentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}