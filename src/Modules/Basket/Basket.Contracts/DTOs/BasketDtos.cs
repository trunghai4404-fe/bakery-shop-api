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
    double? Latitude = null,
    double? Longitude = null
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
    List<BasketItemWarningDto>? Warnings = null,
    string? ImageUrl = null
);

public record BasketDto(
    Guid Id,
    Guid? UserId,
    string? SessionId,
    string FulfillmentType,
    CustomerInfoDto CustomerInfo,
    ShippingInfoDto? ShippingInfo,
    string PaymentMethod,
    string? ShippingMethod,
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
    string? FulfillmentType = null,
    string? PaymentMethod = null,
    string? ShippingMethod = null,
    List<BasketItemRequest>? Items = null,
    string? Note = null
);

public record UpdateBasketRequest(
    string FulfillmentType,
    CustomerInfoDto? CustomerInfo,
    ShippingInfoDto? ShippingInfo,
    string? PaymentMethod,
    string? ShippingMethod,
    List<BasketItemRequest> Items,
    string? Note
);

public record CheckoutResultDto(
    string OrderCode,
    decimal TotalAmount,
    string PaymentStatus
);
