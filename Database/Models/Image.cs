using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using ZapMe.Constants;

namespace ZapMe.Database.Models;

public sealed class ImageEntity
{
    public Guid Id { get; set; }

    public uint Height { get; set; }

    public uint Width { get; set; }

    public uint FrameCount { get; set; }

    public uint SizeBytes { get; set; }

    public required string MimeType { get; set; }

    public required string Sha256 { get; set; }

    public required string R2RegionName { get; set; }

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