namespace Basket.Domain.ValueObjects;

public record ShippingInfo(
    string Address,
    int ProvinceId,
    int WardId,
    double? Latitude,
    double? Longitude,
    string? Note = null
);
