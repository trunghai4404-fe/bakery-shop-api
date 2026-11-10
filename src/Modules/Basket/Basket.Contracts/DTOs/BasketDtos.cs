namespace Basket.Contracts.DTOs;

public record CustomerInfoDto(
    string Name,
    string Phone,
    string Email
);

public record ShippingInfoDto(
    string Address,
    int ProvinceId,
    int WardId,
    double? Latitude,
    double? Longitude,
    string? Note
);

public record BasketItemWarningDto(
    string Code,
    string Message
);

public record BasketItemDto(
    long ProductId,
    string ProductName,
    long VariantId,
    string VariantName,
    int Quantity,
    decimal Price,
    decimal DiscountPrice,
    decimal DiscountPercent,
    decimal FinalPrice,
    Dictionary<string, object>? Promotion,
    List<object>? Gifts,
    List<BasketItemWarningDto>? Warnings = null
);

public record BasketDto(
    Guid Id,
    long? CustomerId,
    string? SessionId,
    string FulfillmentType,
    CustomerInfoDto CustomerInfo,
    ShippingInfoDto? ShippingInfo,
    List<BasketItemDto> Items,
    decimal Subtotal,
    string? Note,
    DateTimeOffset ExpiresAt,
    bool HasWarnings = false
);

public record BasketItemRequest(
    long ProductId,
    long VariantId,
    int Quantity
);

public record CreateBasketRequest(
    string? FulfillmentType,
    List<BasketItemRequest>? Items,
    string? Note
);

public record UpdateBasketRequest(
    string FulfillmentType,
    CustomerInfoDto? CustomerInfo,
    ShippingInfoDto? ShippingInfo,
    List<BasketItemRequest> Items,
    string? Note
);

public record CheckoutBasketRequest(
    string PaymentMethod,
    string? Note
);

public record CheckoutResultDto(
    Guid OrderId,
    string OrderCode,
    decimal TotalAmount,
    string Status
);
