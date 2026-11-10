using SharedKernel.Domain.Entities;

namespace Ordering.Domain.Entities;

public class OrderItem : AuditableEntity<long>
{
    public Guid OrderId { get; private set; }
    public long ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public long VariantId { get; private set; }
    public string VariantName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public decimal DiscountPrice { get; private set; }
    public decimal DiscountPercent { get; private set; }
    public decimal FinalPrice { get; private set; }
    public decimal TotalItemPrice => FinalPrice * Quantity;

    private OrderItem() { }

    public OrderItem(
        long productId,
        string productName,
        long variantId,
        string variantName,
        int quantity,
        decimal price,
        decimal discountPrice,
        decimal discountPercent,
        decimal finalPrice)
    {
        ProductId = productId;
        ProductName = productName;
        VariantId = variantId;
        VariantName = variantName;
        Quantity = quantity;
        Price = price;
        DiscountPrice = discountPrice;
        DiscountPercent = discountPercent;
        FinalPrice = finalPrice;
    }
}
