using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using ZapMe.Constants;

namespace ZapMe.Database.Models;

public sealed class ImageEntity
{
    public const string TableName = "images";
    public const string TableSha256Index = TableName + "_sha256_idx";

    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public uint Height { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public uint Width { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public uint FrameCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public uint SizeBytes { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string MimeType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string Sha256 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string R2RegionName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid? UploaderId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public UserEntity? Uploader { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [NotMapped]
    public Uri PublicUrl => new Uri($"https://r2-{R2RegionName}.{App.Domain}/img_{Id}");
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

        builder.Property(i => i.FrameCount)
            .HasColumnName("frameCount");

        builder.Property(i => i.SizeBytes)
            .HasColumnName("sizeBytes");

        builder.Property(i => i.MimeType)
            .HasColumnName("mimeType")
            .HasMaxLength(32);

        builder.Property(i => i.Sha256)
            .HasColumnName("sha256")
            .HasMaxLength(HashConstants.Sha256LengthHex);

        builder.Property(i => i.R2RegionName)
            .HasColumnName("r2RegionName")
            .HasMaxLength(32);

        builder.Property(i => i.UploaderId)
            .HasColumnName("uploaderId");

        builder.HasOne(i => i.Uploader)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(i => i.Sha256)
            .HasDatabaseName(ImageEntity.TableSha256Index)
            .IsUnique();
    }
}