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
    decimal FinalPrice,
    string? ProductImageUrl = null
);

public record CreateOrderFromBasketRequest(
    Guid? UserId,
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
    long OrderId,
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
    string? ProductImageUrl,
    int Quantity,
    decimal Price,
    decimal DiscountPrice,
    decimal DiscountPercent,
    decimal FinalPrice,
    decimal TotalItemPrice
);

public record OrderDto(
    long Id,
    string OrderCode,
    Guid? UserId,
    string? SessionId,
    string FulfillmentType,
    OrderCustomerInfoDto CustomerInfo,
    OrderShippingAddressDto? ShippingAddress,
    string? ShippingMethodCode,
    string? ShippingMethodName,
    decimal ShippingFee,
    string? ShippingFeeNote,
    string PaymentMethodCode,
    string PaymentMethodName,
    string PaymentStatus,
    string Status,
    decimal Subtotal,
    decimal DiscountAmount,
    decimal TotalAmount,
    string? Note,
    string? CancelReason,
    DateTime? CancelledAt,
    DateTime CreatedAt,
    List<OrderItemDto> Items
);

// DTOs dành cho User
public record UserOrderItemSummaryDto(
    long ProductId,
    string ProductName,
    long VariantId,
    string VariantName,
    string? ProductImageUrl,
    int Quantity,
    decimal FinalPrice,
    decimal TotalItemPrice
);

public record UserOrderSummaryDto(
    string OrderCode,
    DateTime CreatedAt,
    string Status,
    string PaymentStatus,
    string PaymentMethodCode,
    string PaymentMethodName,
    string? ShippingMethodCode,
    string? ShippingMethodName,
    string FulfillmentType,
    decimal TotalAmount,
    int TotalItems,
    List<UserOrderItemSummaryDto> Items
);

public record OrderSummaryDto(
    int All,
    int Pending,
    int Confirmed,
    int Preparing,
    int ReadyForPickup,
    int Shipping,
    int Completed,
    int Cancelled
);

// Backward-compatible alias for user summary count
public record UserOrderSummaryCountDto(
    int All,
    int Pending,
    int Confirmed,
    int Preparing,
    int ReadyForPickup,
    int Shipping,
    int Completed,
    int Cancelled
);

public record CancelOrderRequest(
    string Reason
);

public record PublicLookupOrderRequest(
    string OrderCode,
    string PhoneNumber
);

public record PublicOrderLookupDto(
    string OrderCode,
    string Status,
    string PaymentStatus,
    string PaymentMethodCode,
    string PaymentMethodName,
    string? ShippingMethodCode,
    string? ShippingMethodName,
    string FulfillmentType,
    string MaskedCustomerName,
    string MaskedPhoneNumber,
    string? MaskedAddress,
    decimal Subtotal,
    decimal ShippingFee,
    decimal TotalAmount,
    string? Note,
    string? CancelReason,
    DateTime CreatedAt,
    List<OrderItemDto> Items
);

// DTOs dành cho Admin (CMS)
public record AdminUpdateOrderStatusRequest(
    string Status,
    string? CancelReason = null
);

public record AdminUpdateOrderPaymentStatusRequest(
    string PaymentStatus
);

public record AdminOrderStatisticsDto(
    int TotalOrders,
    int PendingCount,
    int ConfirmedCount,
    int PreparingCount,
    int ReadyForPickupCount,
    int ShippingCount,
    int CompletedCount,
    int CancelledCount,
    decimal TodayRevenue,
    decimal MonthRevenue
);
