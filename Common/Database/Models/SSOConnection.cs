using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Database.Models;

public sealed class SSOConnectionEntity
{
    public const string TableName = "ssoConnections";

    /// <summary>
    /// The id of the user that owns this connection
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Lowercase name of the provider
    /// </summary>
    public required string ProviderName { get; init; }

    /// <summary>
    /// Id of the user on the provider's platform
    /// </summary>
    public required string ProviderUserId { get; init; }

    /// <summary>
    /// Name of the user on the provider's platform
    /// </summary>
    public required string ProviderUserName { get; set; }

    public DateTime CreatedAt { get; private set; }

    public UserEntity User { get; private set; } = null!;
}

public sealed class SSOConnectionEntityConfiguration : IEntityTypeConfiguration<SSOConnectionEntity>
{
    public void Configure(EntityTypeBuilder<SSOConnectionEntity> builder)
    {
        // User can have multiple connections to the same provider, but user's can't share the same connection
        builder.HasKey(oac => new { oac.ProviderName, oac.ProviderUserId });
        builder.Property(oac => oac.ProviderName).HasMaxLength(16);
        builder.Property(oac => oac.CreatedAt).HasDefaultValueSql("now()");
        builder.HasIndex(oac => oac.UserId);
    }
}