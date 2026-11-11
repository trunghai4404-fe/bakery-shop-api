namespace Basket.Domain.ValueObjects;

public record BasketItem(
    long ProductId,
    string ProductName,
    long VariantId,
    string VariantName,
    int Quantity,
    decimal Price,
    decimal DiscountPrice,
    decimal DiscountPercent,
    decimal FinalPrice,
    Dictionary<string, object>? Promotion = null,
    List<object>? Gifts = null,
    string? ImageUrl = null
)
{
    public decimal TotalItemPrice => FinalPrice * Quantity;
}
