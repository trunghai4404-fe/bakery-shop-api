using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("ProductCategory");
        builder.HasKey(x => new { x.ProductId, x.CategoryId });

        builder.HasOne(p => p.Product)
            .WithMany(p => p.ProductCategories)
            .HasForeignKey(x => x.ProductId);
        
        builder.HasOne(p => p.Category)
            .WithMany(c => c.ProductCategories)
            .HasForeignKey(x => x.CategoryId);
    }
}