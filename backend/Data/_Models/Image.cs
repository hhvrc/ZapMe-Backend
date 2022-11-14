using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Data.Models;

public sealed class ImageEntity
{
    public Guid Id { get; set; }

    public int Height { get; set; }
    public int Width { get; set; }

    public int SizeBytes { get; set; }

    public required string HashSha256 { get; set; }
    public required long HashPerceptual { get; set; }

    public Guid? UploaderId { get; set; }
    public UserEntity? Uploader { get; set; }
}

public sealed class ImageEntityConfiguration : IEntityTypeConfiguration<ImageEntity>
{
    public void Configure(EntityTypeBuilder<ImageEntity> builder)
    {
        builder.ToTable("images");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Height)
            .HasColumnName("height");

        builder.Property(i => i.Width)
            .HasColumnName("width");

        builder.Property(i => i.SizeBytes)
            .HasColumnName("sizeBytes");

        builder.Property(i => i.HashSha256)
            .HasColumnName("sha256")
            .HasMaxLength(64);

        builder.Property(i => i.HashPerceptual)
            .HasColumnName("phash");

        builder.Property(i => i.UploaderId)
            .HasColumnName("uploaderId");

        builder.HasOne(i => i.Uploader)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
    }
}