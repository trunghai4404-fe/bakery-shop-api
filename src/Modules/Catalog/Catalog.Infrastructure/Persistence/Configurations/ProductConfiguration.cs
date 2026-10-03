using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).UseIdentityByDefaultColumn();

        builder.Property(p => p.Name).IsRequired().HasMaxLength(256);
        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(256);
        builder.HasIndex(p => p.Slug)
            .IsUnique();

        builder.Property(p => p.Sku)
            .IsRequired()
            .HasMaxLength(250);
        builder.HasIndex(p => p.Sku)
            .IsUnique();

        builder.Property(p => p.Description)
            .IsRequired(false)
            .HasMaxLength(50000);
        builder.Property(p => p.IsActive).IsRequired().HasDefaultValue(false);

        builder.HasMany(p => p.ProductImages)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(p => p.ProductVariants)
            .WithOne(p => p.Product)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}