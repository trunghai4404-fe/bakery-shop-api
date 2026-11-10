namespace Ordering.Domain.ValueObjects;

public record OrderShippingAddress(
    string Address,
    int ProvinceId,
    int WardId,
    double? Latitude = null,
    double? Longitude = null
);
