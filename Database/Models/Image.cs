using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using ZapMe.Constants;

namespace ZapMe.Database.Models;

public sealed class ImageEntity
{
    public Guid Id { get; private set; }

    public uint Height { get; init; }

    public uint Width { get; init; }

    public uint FrameCount { get; init; }

    public uint SizeBytes { get; init; }

    public required string MimeType { get; init; }

    public required string Sha256 { get; init; }

    public required string R2RegionName { get; init; }

    public Guid? UploaderId { get; set; }

    public UserEntity? Uploader { get; private set; } = null;

    [NotMapped]
    public Uri PublicUrl => new Uri($"https://r2-{R2RegionName}.{App.Domain}/img_{Id}");
}

public sealed class ImageEntityConfiguration : IEntityTypeConfiguration<ImageEntity>
{
    public void Configure(EntityTypeBuilder<ImageEntity> builder)
    {
        builder.Property(i => i.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(i => i.MimeType).HasMaxLength(32);
        builder.Property(i => i.Sha256).HasMaxLength(HashConstants.Sha256LengthHex);
        builder.Property(i => i.R2RegionName).HasMaxLength(32);
        builder.HasIndex(i => i.Sha256).IsUnique();

        builder.HasOne(i => i.Uploader)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
    }
}