namespace Ordering.Domain.ValueObjects;

public record OrderStoreInfo(
    long Id,
    string Name,
    string Address,
    int ProvinceId,
    int WardId,
    double Latitude,
    double Longitude,
    string? Phone,
    string? Email,
    string? OpeningHours
);
