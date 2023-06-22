using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;

namespace ZapMe.Database.Models;

public sealed class SessionEntity
{
    public Guid Id { get; private set; }

    public Guid UserId { get; init; }

    /// <summary>
    /// User provided name for this session
    /// </summary>
    public string? NickName { get; set; }

    /// <summary>
    /// The visitor's IP address (IPv4 or IPv6)
    /// This is used to prevent session hijacking
    /// The source of this value is fetched from the cloudflare provided headers, forwarded for headers, or the remote ip address
    /// </summary>
    public required string IpAddress { get; init; }

    /// <summary>
    /// The country code of the visitor's IP address (in ISO 3166-1 Alpha 2 format)
    /// This is used to prevent session hijacking
    /// If unknown, this value will be set to ZZ
    /// </summary>
    public required string CountryCode { get; init; }

    public Guid UserAgentId { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime ExpiresAt { get; set; }

    public UserEntity User { get; private set; } = null!;
    public UserAgentEntity UserAgent { get; private set; } = null!;
}

public sealed class SessionEntityConfiguration : IEntityTypeConfiguration<SessionEntity>
{
    public void Configure(EntityTypeBuilder<SessionEntity> builder)
    {
        builder.Property(si => si.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(si => si.NickName).HasMaxLength(32);
        builder.Property(si => si.IpAddress).HasMaxLength(GeneralHardLimits.IPAddressMaxLength);
        builder.Property(si => si.CountryCode).HasMaxLength(2);
        builder.Property(si => si.CreatedAt).HasDefaultValueSql("now()");
    }
}