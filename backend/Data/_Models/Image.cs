using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using ZapMe.Constants;

namespace ZapMe.Data.Models;

public sealed class ImageEntity
{
    public const string TableName = "images";
    public const string TableSha256Index = TableName + "_sha256_idx";
    public static readonly Guid DefaultImageId = Guid.Parse("00000000-0000-0000-0000-000000000001");

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
    public required string Extension { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string Sha256 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string S3BucketName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string S3RegionName { get; set; }

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
    public Uri PublicUrl => new Uri($"https://{S3BucketName}.s3.{S3RegionName}.amazonaws.com/img_{Id}");
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

        builder.Property(i => i.Extension)
            .HasColumnName("extension")
            .HasMaxLength(8);

        builder.Property(i => i.Sha256)
            .HasColumnName("sha256")
            .HasMaxLength(HashConstants.Sha256LengthHex);

        builder.Property(i => i.S3BucketName)
            .HasColumnName("s3BucketName")
            .HasMaxLength(32);

        builder.Property(i => i.S3RegionName)
            .HasColumnName("s3RegionName")
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