using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Database.Models;

public sealed class PrivacyPolicyDocumentEntity
{
    public uint Version { get; set; }

    public required string Text { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }
}

public sealed class PrivacyPolicyDocumentEntityConfiguration : IEntityTypeConfiguration<PrivacyPolicyDocumentEntity>
{
    public void Configure(EntityTypeBuilder<PrivacyPolicyDocumentEntity> builder)
    {
        builder.HasKey(pp => pp.Version);
        builder.Property(pp => pp.Version).ValueGeneratedOnAdd();
        builder.Property(pp => pp.CreatedAt).HasDefaultValueSql("now()");
    }
}