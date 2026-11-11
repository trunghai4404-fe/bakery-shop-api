using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedOnAdd();

        builder.Property(i => i.ProductName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(i => i.VariantName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(i => i.ProductImageUrl)
            .HasMaxLength(500);

        builder.Property(i => i.Price)
            .HasPrecision(18, 2);

        builder.Property(i => i.DiscountPrice)
            .HasPrecision(18, 2);

        builder.Property(i => i.DiscountPercent)
            .HasPrecision(5, 2);

        builder.Property(i => i.FinalPrice)
            .HasPrecision(18, 2);

        builder.Ignore(i => i.TotalItemPrice);
    }
}
