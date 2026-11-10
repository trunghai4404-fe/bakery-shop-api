using System.Text.Json;
using Basket.Domain.Entities;
using Basket.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Basket.Infrastructure.Persistence.Configurations;

public class BasketConfiguration : IEntityTypeConfiguration<Basket.Domain.Entities.Basket>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public void Configure(EntityTypeBuilder<Basket.Domain.Entities.Basket> builder)
    {
        builder.ToTable("Baskets");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever();

        builder.Property(b => b.CustomerId);

        builder.Property(b => b.SessionId)
            .HasMaxLength(100);

        builder.Property(b => b.FulfillmentType)
            .IsRequired();

        builder.Property(b => b.PaymentMethod)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("COD");

        builder.Property(b => b.ShippingMethod)
            .HasMaxLength(50);

        builder.Property(b => b.Note)
            .HasMaxLength(1000);

        builder.Property(b => b.ExpiresAt)
            .IsRequired();

        builder.Property(b => b.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Bỏ qua derived property không lưu vào cột riêng
        builder.Ignore(b => b.Subtotal);

        // Lưu CustomerInfo dạng JSONB
        builder.Property(b => b.CustomerInfo)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonOptions),
                v => JsonSerializer.Deserialize<CustomerInfo>(v, JsonOptions) ?? CustomerInfo.Empty);

        // Lưu ShippingInfo dạng JSONB (có thể null)
        builder.Property(b => b.ShippingInfo)
            .HasColumnType("jsonb")
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, JsonOptions),
                v => string.IsNullOrEmpty(v) ? null : JsonSerializer.Deserialize<ShippingInfo>(v, JsonOptions));

        // Lưu Items dạng JSONB
        builder.Property(b => b.Items)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonOptions),
                v => JsonSerializer.Deserialize<List<BasketItem>>(v, JsonOptions) ?? new List<BasketItem>());

        // Partial Unique Index: 1 Customer chỉ có tối đa 1 Active Basket
        builder.HasIndex(b => b.CustomerId)
            .IsUnique()
            .HasFilter("\"CustomerId\" IS NOT NULL AND \"IsActive\" = true");

        // Partial Unique Index: 1 Session chỉ có tối đa 1 Active Basket
        builder.HasIndex(b => b.SessionId)
            .IsUnique()
            .HasFilter("\"SessionId\" IS NOT NULL AND \"IsActive\" = true");

        // Index phục vụ background job dọn dẹp giỏ hàng hết hạn
        builder.HasIndex(b => b.ExpiresAt);
    }
}
