using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class ProductAccessoryConfiguration : IEntityTypeConfiguration<ProductAccessory>
{
    public void Configure(EntityTypeBuilder<ProductAccessory> builder)
    {
        builder.ToTable("ProductAccessories");
        builder.HasKey(pa => new { pa.ProductId, pa.AccessoryProductId });

        builder.Property(pa => pa.DefaultQuantity).IsRequired().HasDefaultValue(1);
        builder.Property(pa => pa.IsIncluded).IsRequired().HasDefaultValue(true);
        builder.Property(pa => pa.SortOrder).IsRequired().HasDefaultValue(0);
        builder.Property(pa => pa.IsMultiSelect).IsRequired().HasDefaultValue(false);
        builder.Property(pa => pa.MaxSelectCount).IsRequired(false);
        builder.Property(pa => pa.IsRequired).IsRequired().HasDefaultValue(false);

        builder.HasOne(pa => pa.Product)
            .WithMany(p => p.ProductAccessories)
            .HasForeignKey(pa => pa.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pa => pa.AccessoryProduct)
            .WithMany()
            .HasForeignKey(pa => pa.AccessoryProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
