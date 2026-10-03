using Amazon.S3.Model;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).UseIdentityByDefaultColumn();
        
        builder.Property(p => p.ProductId).IsRequired();
        builder.Property(p => p.DesktopUrl).HasMaxLength(1000);
        builder.Property(p => p.MobileUrl).HasMaxLength(1000);
        builder.Property(p => p.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property(p => p.SortOrder);

    }
    
}