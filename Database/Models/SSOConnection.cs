using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Database.Models;

public sealed class SSOConnectionEntity
{
    public const string TableName = "ssoConnections";

    /// <summary>
    /// The id of the user that owns this connection
    /// </summary>
    public Guid UserId { get; set; }
    public required UserEntity User { get; set; }

    /// <summary>
    /// Lowercase name of the provider
    /// </summary>
    public required string ProviderName { get; set; }

    /// <summary>
    /// Id of the user on the provider's platform
    /// </summary>
    public required string ProviderUserId { get; set; }

    /// <summary>
    /// Name of the user on the provider's platform
    /// </summary>
    public required string ProviderUserName { get; set; }

    public DateTime CreatedAt { get; set; }
}

public sealed class SSOConnectionEntityConfiguration : IEntityTypeConfiguration<SSOConnectionEntity>
{
    public void Configure(EntityTypeBuilder<SSOConnectionEntity> builder)
    {
        builder.ToTable(SSOConnectionEntity.TableName);

        // User can have multiple connections to the same provider, but user's can't share the same connection
        builder.HasKey(oac => new { oac.ProviderName, oac.ProviderUserId });

        builder.Property(oac => oac.UserId)
            .HasColumnName("userId");

        builder.Property(oac => oac.ProviderName)
            .HasColumnName("providerName")
            .HasMaxLength(16);

        builder.Property(oac => oac.ProviderUserId)
            .HasColumnName("providerUserId");

        builder.Property(oac => oac.ProviderUserName)
            .HasColumnName("providerUserName");

        builder.Property(oac => oac.CreatedAt)
            .HasColumnName("createdAt")
            .HasDefaultValueSql("now()");

        builder.HasOne(oac => oac.User)
            .WithMany(u => u.SSOConnections)
            .HasForeignKey(oac => oac.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Fast user lookup, we don't need a index for provider name+id because the key is already indexed
        builder.HasIndex(oac => oac.UserId);
    }
}