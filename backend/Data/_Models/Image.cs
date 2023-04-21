using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZapMe.Constants;

namespace ZapMe.Data.Models;

public sealed class ImageEntity
{
    public const string TableName = "images";
    public const string TableSha256Index = TableName + "_sha256_idx";
    public const string TableHashPerceptualIndex = TableName + "_phash_idx";
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
    public uint SizeBytes { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public required string Sha256 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ulong HashPerceptual { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid? UploaderId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public UserEntity? Uploader { get; set; }
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

        builder.Property(i => i.Sha256)
            .HasColumnName("sha256")
            .HasMaxLength(HashConstants.Sha256LengthHex);

        builder.Property(i => i.HashPerceptual)
            .HasColumnName("phash");

        builder.Property(i => i.UploaderId)
            .HasColumnName("uploaderId");

        builder.HasOne(i => i.Uploader)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(i => i.Sha256)
            .HasDatabaseName(ImageEntity.TableSha256Index)
            .IsUnique();

        builder.HasIndex(i => i.HashPerceptual)
            .HasDatabaseName(ImageEntity.TableHashPerceptualIndex)
        //    .IsUnique()
            ;
    }
}