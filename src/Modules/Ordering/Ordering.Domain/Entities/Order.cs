using Ordering.Domain.Entities;
using Ordering.Domain.Enums;
using Ordering.Domain.Errors;
using Ordering.Domain.ValueObjects;
using SharedKernel.Domain;
using SharedKernel.Domain.Entities;

namespace Ordering.Domain.Entities;

public class Order : AuditableEntity<long>
{
    private const string OrderCodeChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public string OrderCode { get; private set; } = string.Empty;
    public Guid? UserId { get; private set; }
    public string? SessionId { get; private set; }
    public string FulfillmentType { get; private set; } = "Pickup"; // "Pickup" | "Delivery"

    public OrderCustomerInfo CustomerInfo { get; private set; } = default!;
    public OrderShippingAddress? ShippingAddress { get; private set; }

    public string? ShippingMethodCode { get; private set; } // "INTERNAL", "GHN"...
    public string? ShippingMethodName { get; private set; } // Label e.g. "Cửa hàng tự giao"
    public decimal ShippingFee { get; private set; } = 0;
    public string? ShippingFeeNote { get; private set; }

    public string PaymentMethodCode { get; private set; } = "COD"; // "COD", "VNPAY"...
    public string PaymentMethodName { get; private set; } = "Thanh toán khi nhận hàng (COD)"; // Label
    public PaymentStatus PaymentStatus { get; private set; } = PaymentStatus.Unpaid;
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public decimal Subtotal { get; private set; }
    public decimal DiscountAmount { get; private set; } = 0;
    public decimal TotalAmount { get; private set; }

    public string? Note { get; private set; }
    public string? CancelReason { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    public List<OrderItem> Items { get; private set; } = [];

    private Order() { }

    public static Order Create(
        Guid? userId,
        string? sessionId,
        string fulfillmentType,
        OrderCustomerInfo customerInfo,
        OrderShippingAddress? shippingAddress,
        string? shippingMethodCode,
        string? shippingMethodName,
        decimal shippingFee,
        string? shippingFeeNote,
        string paymentMethodCode,
        string paymentMethodName,
        List<OrderItem> items,
        string? note,
        decimal discountAmount = 0)
    {
        var orderCode = GenerateOrderCode();
        var subtotal = items.Sum(i => i.TotalItemPrice);
        var totalAmount = subtotal + shippingFee - discountAmount;

        var order = new Order
        {
            OrderCode = orderCode,
            UserId = userId,
            SessionId = sessionId,
            FulfillmentType = fulfillmentType,
            CustomerInfo = customerInfo,
            ShippingAddress = shippingAddress,
            ShippingMethodCode = shippingMethodCode,
            ShippingMethodName = shippingMethodName,
            ShippingFee = shippingFee,
            ShippingFeeNote = shippingFeeNote,
            PaymentMethodCode = paymentMethodCode,
            PaymentMethodName = paymentMethodName,
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

    public Result UpdateOrderStatus(OrderStatus status, string? cancelReason = null)
    {
        if (status == OrderStatus.Cancelled)
        {
            return CancelByAdmin(cancelReason ?? "Admin hủy đơn hàng");
        }

        if (Status == OrderStatus.Completed)
        {
            return Result.Failure(OrderingErrors.OrderAlreadyCompleted);
        }

        if (Status == OrderStatus.Cancelled)
        {
            return Result.Failure(OrderingErrors.OrderAlreadyCancelled);
        }

        Status = status;
        if (status == OrderStatus.Completed && PaymentMethodCode == "COD" && PaymentStatus == PaymentStatus.Unpaid)
        {
            PaymentStatus = PaymentStatus.Paid;
        }
        LastModifiedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result CancelByCustomer(string reason)
    {
        if (Status != OrderStatus.Pending)
        {
            return Result.Failure(OrderingErrors.CannotCancelNonPendingOrder);
        }

        Status = OrderStatus.Cancelled;
        CancelReason = reason;
        CancelledAt = DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result CancelByAdmin(string reason)
    {
        if (Status == OrderStatus.Completed)
        {
            return Result.Failure(OrderingErrors.OrderAlreadyCompleted);
        }

        if (Status == OrderStatus.Cancelled)
        {
            return Result.Failure(OrderingErrors.OrderAlreadyCancelled);
        }

        Status = OrderStatus.Cancelled;
        CancelReason = reason;
        CancelledAt = DateTime.UtcNow;
        LastModifiedAt = DateTime.UtcNow;
        return Result.Success();
    }
}
