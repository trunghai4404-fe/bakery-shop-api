using Basket.Domain.Enums;
using Basket.Domain.ValueObjects;
using SharedKernel.Domain.Entities;

namespace Basket.Domain.Entities;

public class Basket : AuditableEntity<Guid>
{
    public long? CustomerId { get; private set; }
    public string? SessionId { get; private set; }
    public FulfillmentType FulfillmentType { get; private set; } = FulfillmentType.Pickup;
    public CustomerInfo CustomerInfo { get; private set; } = CustomerInfo.Empty;
    public ShippingInfo? ShippingInfo { get; private set; }
    public List<BasketItem> Items { get; private set; } = [];
    public string? Note { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Derived Property: Tổng tiền tạm tính toàn bộ giỏ
    public decimal Subtotal => Items.Sum(i => i.TotalItemPrice);

    private Basket() { }

    // Factory method tạo giỏ hàng mới
    public static Basket Create(long? customerId, string? sessionId, int expiryDays = 30)
    {
        return new Basket
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            SessionId = customerId.HasValue ? null : sessionId,
            FulfillmentType = FulfillmentType.Pickup,
            CustomerInfo = CustomerInfo.Empty,
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

    // Nghiệp vụ: Cập nhật hình thức giao nhận & thông tin khách
    public void UpdateFulfillment(
        FulfillmentType fulfillmentType, 
        CustomerInfo customerInfo, 
        ShippingInfo? shippingInfo, 
        string? note)
    {
        FulfillmentType = fulfillmentType;
        CustomerInfo = customerInfo;
        ShippingInfo = fulfillmentType == FulfillmentType.Delivery ? shippingInfo : null;
        Note = note;
        LastModifiedAt = DateTime.UtcNow;
    }

    // Nghiệp vụ: Gán CustomerId khi khách đăng nhập (Merge giỏ hàng)
    public void AssignToCustomer(long customerId)
    {
        CustomerId = customerId;
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