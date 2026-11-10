namespace Ordering.Contracts.DTOs;

public record PaymentMethodDto(
    string Code,
    string Name,
    string? Description,
    string? IconUrl,
    bool IsEnabled
);

public record TogglePaymentMethodRequest(bool? IsEnabled = null);
public record TogglePaymentMethodResponse(string Code, bool IsEnabled);

public record ShippingMethodDto(
    string Code,
    string Name,
    decimal Fee,
    string? FeeDescription,
    string? EstimatedDeliveryTime,
    bool IsEnabled
);

public record ToggleShippingMethodRequest(bool? IsEnabled = null);
public record ToggleShippingMethodResponse(string Code, bool IsEnabled);

public record OrderCustomerInfoDto(
    string Name,
    string Phone,
    string Email
);

public record OrderShippingAddressDto(
    string Address,
    int ProvinceId,
    int WardId,
    double? Latitude = null,
    double? Longitude = null
);

public record CreateOrderItemRequest(
    long ProductId,
    string ProductName,
    long VariantId,
    string VariantName,
    int Quantity,
    decimal Price,
    decimal DiscountPrice,
    decimal DiscountPercent,
    decimal FinalPrice
);

public record CreateOrderFromBasketRequest(
    long? CustomerId,
    string? SessionId,
    string FulfillmentType,
    OrderCustomerInfoDto CustomerInfo,
    OrderShippingAddressDto? ShippingAddress,
    string PaymentMethod,
    string? ShippingMethod,
    List<CreateOrderItemRequest> Items,
    string? Note
);

public record CreateOrderResultDto(
    Guid OrderId,
    string OrderCode,
    decimal TotalAmount,
    string Status,
    string PaymentStatus
);

public record OrderItemDto(
    long Id,
    long ProductId,
    string ProductName,
    long VariantId,
    string VariantName,
    int Quantity,
    decimal Price,
    decimal DiscountPrice,
    decimal DiscountPercent,
    decimal FinalPrice,
    decimal TotalItemPrice
);

public record OrderDto(
    Guid Id,
    string OrderCode,
    long? CustomerId,
    string? SessionId,
    string FulfillmentType,
    OrderCustomerInfoDto CustomerInfo,
    OrderShippingAddressDto? ShippingAddress,
    string? ShippingMethodCode,
    decimal ShippingFee,
    string? ShippingFeeNote,
    string PaymentMethodCode,
    string PaymentStatus,
    string Status,
    decimal Subtotal,
    decimal DiscountAmount,
    decimal TotalAmount,
    string? Note,
    DateTime CreatedAt,
    List<OrderItemDto> Items
);
