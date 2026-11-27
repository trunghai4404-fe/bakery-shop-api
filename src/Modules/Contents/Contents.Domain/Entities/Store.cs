using SharedKernel.Domain.Entities;
using SharedKernel.Domain.Interfaces;

namespace Contents.Domain.Entities;

public class Store : AuditableEntity<long>, ISoftDelete
{
    public string Name { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public int ProvinceId { get; private set; }
    public int WardId { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? OpeningHours { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int SortOrder { get; private set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    private Store() { }

    public static Store Create(
        string name,
        string address,
        int provinceId,
        int wardId,
        double latitude,
        double longitude,
        string? phone,
        string? email,
        string? openingHours,
        bool isActive,
        int sortOrder
    )
    {
        return new Store
        {
            Name = name.Trim(),
            Address = address.Trim(),
            ProvinceId = provinceId,
            WardId = wardId,
            Latitude = latitude,
            Longitude = longitude,
            Phone = phone?.Trim(),
            Email = email?.Trim(),
            OpeningHours = openingHours?.Trim(),
            IsActive = isActive,
            SortOrder = sortOrder
        };
    }

    public void Update(
        string name,
        string address,
        int provinceId,
        int wardId,
        double latitude,
        double longitude,
        string? phone,
        string? email,
        string? openingHours,
        bool isActive,
        int sortOrder
    )
    {
        Name = name.Trim();
        Address = address.Trim();
        ProvinceId = provinceId;
        WardId = wardId;
        Latitude = latitude;
        Longitude = longitude;
        Phone = phone?.Trim();
        Email = email?.Trim();
        OpeningHours = openingHours?.Trim();
        IsActive = isActive;
        SortOrder = sortOrder;
    }

    public void ToggleStatus(bool? isActive = null) => IsActive = isActive ?? !IsActive;
}
