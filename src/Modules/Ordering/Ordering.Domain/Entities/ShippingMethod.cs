using SharedKernel.Domain.Entities;

namespace Ordering.Domain.Entities;

public class ShippingMethod : AuditableEntity<int>
{
    public string Code { get; private set; } = string.Empty; // "INTERNAL", "GHN", "AHAMOVE"
    public string Name { get; private set; } = string.Empty;
    public decimal Fee { get; private set; } = 0;
    public string? FeeDescription { get; private set; }
    public string? EstimatedDeliveryTime { get; private set; }
    public bool IsEnabled { get; private set; } = true;
    public int DisplayOrder { get; private set; } = 0;

    private ShippingMethod() { }

    public ShippingMethod(
        int id,
        string code,
        string name,
        decimal fee,
        string? feeDescription,
        string? estimatedDeliveryTime,
        bool isEnabled,
        int displayOrder)
    {
        Id = id;
        Code = code;
        Name = name;
        Fee = fee;
        FeeDescription = feeDescription;
        EstimatedDeliveryTime = estimatedDeliveryTime;
        IsEnabled = isEnabled;
        DisplayOrder = displayOrder;
    }

    public void UpdateStatus(bool isEnabled)
    {
        IsEnabled = isEnabled;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void ToggleStatus()
    {
        IsEnabled = !IsEnabled;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void UpdateFee(decimal fee, string? feeDescription)
    {
        Fee = fee;
        FeeDescription = feeDescription;
        LastModifiedAt = DateTime.UtcNow;
    }
}
