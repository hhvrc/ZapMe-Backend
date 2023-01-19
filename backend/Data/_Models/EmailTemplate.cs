using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZapMe.Data.Models;

public sealed class EmailTemplateEntity
{
    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public string Body { get; set; } = null!;
}

public sealed class EmailTemplateEntityConfiguration : IEntityTypeConfiguration<EmailTemplateEntity>
{
    public void Configure(EntityTypeBuilder<EmailTemplateEntity> builder)
    {
        builder.ToTable("emailTemplates");

        builder.HasKey(u => u.Name);

        builder.Property(u => u.Name)
            .HasColumnName("name");

        builder.Property(u => u.Body)
            .HasColumnName("body");
    }
}