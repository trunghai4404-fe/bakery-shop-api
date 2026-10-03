using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).UseIdentityByDefaultColumn();
        
        builder.Property(p => p.ProductId)
            .IsRequired();
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(250);
        builder.Property(p => p.Sku)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasIndex(p => p.Sku).IsUnique();
        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        builder.Property(p => p.StockQuantity)
            .IsRequired();
        builder.Property(p => p.IsActive)
            .IsRequired();
        builder.Property(p => p.SortOrder);
    }
}