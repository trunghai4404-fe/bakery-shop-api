namespace Basket.Domain.ValueObjects;

public record ShippingInfo(
    string Address,
    int ProvinceId,
    int WardId,
    double? Latitude = null,
    double? Longitude = null
);
