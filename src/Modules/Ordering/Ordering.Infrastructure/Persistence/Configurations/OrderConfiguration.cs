using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Entities;
using Ordering.Domain.ValueObjects;

namespace Ordering.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).UseIdentityByDefaultColumn();

        builder.Property(o => o.OrderCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.UserId);

        builder.Property(o => o.SessionId)
            .HasMaxLength(100);

        builder.Property(o => o.FulfillmentType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(o => o.ShippingMethodCode)
            .HasMaxLength(50);

        builder.Property(o => o.ShippingMethodName)
            .HasMaxLength(100);

        builder.Property(o => o.ShippingFee)
            .HasPrecision(18, 2);

        builder.Property(o => o.ShippingFeeNote)
            .HasMaxLength(255);

        builder.Property(o => o.PaymentMethodCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.PaymentMethodName)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("Thanh toán khi nhận hàng (COD)");

        builder.Property(o => o.PaymentStatus)
            .IsRequired();

        builder.Property(o => o.Status)
            .IsRequired();

        builder.Property(o => o.Subtotal)
            .HasPrecision(18, 2);

        builder.Property(o => o.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(o => o.Note)
            .HasMaxLength(1000);

        builder.Property(o => o.CancelReason)
            .HasMaxLength(500);

        // Lưu CustomerInfo dạng JSONB
        builder.Property(o => o.CustomerInfo)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonOptions),
                v => JsonSerializer.Deserialize<OrderCustomerInfo>(v, JsonOptions) ?? new OrderCustomerInfo("", "", ""));

        // Lưu ShippingAddress dạng JSONB (có thể null)
        builder.Property(o => o.ShippingAddress)
            .HasColumnType("jsonb")
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, JsonOptions),
                v => string.IsNullOrEmpty(v) ? null : JsonSerializer.Deserialize<OrderShippingAddress>(v, JsonOptions));

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.OrderCode).IsUnique();
        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.CreatedAt);
    }
}
