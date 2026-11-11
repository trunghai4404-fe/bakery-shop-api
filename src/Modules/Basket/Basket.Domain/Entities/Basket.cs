using Basket.Domain.Enums;
using Basket.Domain.ValueObjects;
using SharedKernel.Domain.Entities;

namespace Basket.Domain.Entities;

public class Basket : AuditableEntity<Guid>
{
    public Guid? UserId { get; private set; }
    public string? SessionId { get; private set; }
    public FulfillmentType FulfillmentType { get; private set; } = FulfillmentType.Pickup;
    public CustomerInfo CustomerInfo { get; private set; } = CustomerInfo.Empty;
    public ShippingInfo? ShippingInfo { get; private set; }
    public string PaymentMethod { get; private set; } = "COD";
    public string? ShippingMethod { get; private set; }
    public List<BasketItem> Items { get; private set; } = [];
    public string? Note { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Derived Property: Tổng tiền tạm tính toàn bộ giỏ
    public decimal Subtotal => Items.Sum(i => i.TotalItemPrice);

    private Basket() { }

    // Factory method tạo giỏ hàng mới
    public static Basket Create(Guid? userId, string? sessionId, int expiryDays = 30)
    {
        return new Basket
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SessionId = userId.HasValue ? null : sessionId,
            FulfillmentType = FulfillmentType.Pickup,
            CustomerInfo = CustomerInfo.Empty,
            PaymentMethod = "COD",
            ShippingMethod = null,
            Items = [],
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(expiryDays),
            IsActive = true
        };
    }

    // Nghiệp vụ: Cập nhật toàn bộ items trong giỏ
    public void UpdateItems(List<BasketItem> items)
    {
        Items = items ?? [];
        LastModifiedAt = DateTime.UtcNow;
    }

    // Nghiệp vụ: Cập nhật hình thức giao nhận, thông tin khách, thanh toán, vận chuyển & ghi chú
    public void UpdateFulfillment(
        FulfillmentType fulfillmentType, 
        CustomerInfo customerInfo, 
        ShippingInfo? shippingInfo, 
        string? paymentMethod,
        string? shippingMethod,
        string? note)
    {
        FulfillmentType = fulfillmentType;
        CustomerInfo = customerInfo;
        ShippingInfo = fulfillmentType == FulfillmentType.Delivery ? shippingInfo : null;
        PaymentMethod = string.IsNullOrWhiteSpace(paymentMethod) ? "COD" : paymentMethod;
        ShippingMethod = fulfillmentType == FulfillmentType.Delivery 
            ? (string.IsNullOrWhiteSpace(shippingMethod) ? "INTERNAL" : shippingMethod)
            : null;
        Note = note;
        LastModifiedAt = DateTime.UtcNow;
    }

    // Nghiệp vụ: Gán UserId khi người dùng đăng nhập (Merge giỏ hàng)
    public void AssignToUser(Guid userId)
    {
        UserId = userId;
        SessionId = null;
        LastModifiedAt = DateTime.UtcNow;
    }

    // Nghiệp vụ: Làm sạch giỏ sau khi đặt hàng thành công
    public void Clear()
    {
        Items.Clear();
        Note = null;
        LastModifiedAt = DateTime.UtcNow;
    }

    // Nghiệp vụ: Đóng giỏ hàng
    public void Deactivate()
    {
        IsActive = false;
        LastModifiedAt = DateTime.UtcNow;
    }
}
