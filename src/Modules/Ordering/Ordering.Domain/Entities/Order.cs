using Ordering.Domain.Enums;
using Ordering.Domain.ValueObjects;
using SharedKernel.Domain.Entities;

namespace Ordering.Domain.Entities;

public class Order : AuditableEntity<Guid>
{
    private const string OrderCodeChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public string OrderCode { get; private set; } = string.Empty;
    public long? CustomerId { get; private set; }
    public string? SessionId { get; private set; }
    public string FulfillmentType { get; private set; } = "Pickup"; // "Pickup" | "Delivery"

    public OrderCustomerInfo CustomerInfo { get; private set; } = default!;
    public OrderShippingAddress? ShippingAddress { get; private set; }

    public string? ShippingMethodCode { get; private set; } // "INTERNAL", "GHN"...
    public decimal ShippingFee { get; private set; } = 0;
    public string? ShippingFeeNote { get; private set; }

    public string PaymentMethodCode { get; private set; } = "COD"; // "COD", "VNPAY"...
    public PaymentStatus PaymentStatus { get; private set; } = PaymentStatus.Unpaid;
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public decimal Subtotal { get; private set; }
    public decimal DiscountAmount { get; private set; } = 0;
    public decimal TotalAmount { get; private set; }

    public string? Note { get; private set; }
    public List<OrderItem> Items { get; private set; } = [];

    private Order() { }

    public static Order Create(
        long? customerId,
        string? sessionId,
        string fulfillmentType,
        OrderCustomerInfo customerInfo,
        OrderShippingAddress? shippingAddress,
        string? shippingMethodCode,
        decimal shippingFee,
        string? shippingFeeNote,
        string paymentMethodCode,
        List<OrderItem> items,
        string? note,
        decimal discountAmount = 0)
    {
        var id = Guid.NewGuid();
        var orderCode = GenerateOrderCode();
        var subtotal = items.Sum(i => i.TotalItemPrice);
        var totalAmount = subtotal + shippingFee - discountAmount;

        var order = new Order
        {
            Id = id,
            OrderCode = orderCode,
            CustomerId = customerId,
            SessionId = sessionId,
            FulfillmentType = fulfillmentType,
            CustomerInfo = customerInfo,
            ShippingAddress = shippingAddress,
            ShippingMethodCode = shippingMethodCode,
            ShippingFee = shippingFee,
            ShippingFeeNote = shippingFeeNote,
            PaymentMethodCode = paymentMethodCode,
            PaymentStatus = PaymentStatus.Unpaid,
            Status = OrderStatus.Pending,
            Subtotal = subtotal,
            DiscountAmount = discountAmount,
            TotalAmount = totalAmount,
            Items = items,
            Note = note,
            CreatedAt = DateTime.UtcNow
        };

        return order;
    }

    public static string GenerateOrderCode()
    {
        var randomSuffix = new string(Random.Shared.GetItems(OrderCodeChars.AsSpan(), 6));
        return $"ORD{DateTime.UtcNow:yyyyMMdd}{randomSuffix}";
    }

    public void UpdatePaymentStatus(PaymentStatus status)
    {
        PaymentStatus = status;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void UpdateOrderStatus(OrderStatus status)
    {
        Status = status;
        LastModifiedAt = DateTime.UtcNow;
    }
}
