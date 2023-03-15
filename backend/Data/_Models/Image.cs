using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Data.Models;

public sealed class ImageEntity
{
    public static string TableName => "images";

    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int SizeBytes { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string HashSha256 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required long HashPerceptual { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid? UploaderId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public AccountEntity? Uploader { get; set; }
}

public sealed class ImageEntityConfiguration : IEntityTypeConfiguration<ImageEntity>
{
    public void Configure(EntityTypeBuilder<ImageEntity> builder)
    {
        builder.ToTable(ImageEntity.TableName);

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

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