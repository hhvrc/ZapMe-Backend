using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Database.Models;

public sealed class TermsOfServiceDocumentEntity
{
    public uint Version { get; set; }

    public required string Markdown { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }
}

public sealed class TermsOfServiceDocumentEntityConfiguration : IEntityTypeConfiguration<TermsOfServiceDocumentEntity>
{
    public void Configure(EntityTypeBuilder<TermsOfServiceDocumentEntity> builder)
    {
        builder.HasKey(tos => tos.Version);
        builder.Property(tos => tos.Version).ValueGeneratedOnAdd();
        builder.Property(tos => tos.CreatedAt).HasDefaultValueSql("now()");
    }
}