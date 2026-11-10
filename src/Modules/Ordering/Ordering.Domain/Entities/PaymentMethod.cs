using SharedKernel.Domain.Entities;

namespace Ordering.Domain.Entities;

public class PaymentMethod : AuditableEntity<int>
{
    public string Code { get; private set; } = string.Empty; // "COD", "VNPAY", "MOMO"
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? IconUrl { get; private set; }
    public bool IsEnabled { get; private set; } = true;
    public int DisplayOrder { get; private set; } = 0;

    private PaymentMethod() { }

    public PaymentMethod(
        int id,
        string code,
        string name,
        string? description,
        string? iconUrl,
        bool isEnabled,
        int displayOrder)
    {
        Id = id;
        Code = code;
        Name = name;
        Description = description;
        IconUrl = iconUrl;
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
}
