using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Data.Models;

public sealed class OAuthConnectionEntity
{
    public const string TableName = "oauthConnections";

    /// <summary>
    /// 
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required AccountEntity User { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string ProviderName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string ProviderId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

public sealed class OAuthConnectionEntityConfiguration : IEntityTypeConfiguration<OAuthConnectionEntity>
{
    public void Configure(EntityTypeBuilder<OAuthConnectionEntity> builder)
    {
        builder.ToTable(OAuthConnectionEntity.TableName);

        builder.HasKey(oac => new { oac.UserId, oac.ProviderName });

        builder.Property(oac => oac.UserId)
            .HasColumnName("userId");

        builder.Property(oac => oac.ProviderName)
            .HasColumnName("providerName")
            .HasMaxLength(16);

        builder.Property(oac => oac.ProviderId)
            .HasColumnName("providerId");

        builder.Property(oac => oac.CreatedAt)
            .HasColumnName("createdAt")
            .HasDefaultValueSql("now()");

        builder.HasOne(oac => oac.User)
            .WithMany(u => u.OauthConnections)
            .HasForeignKey(oac => oac.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}