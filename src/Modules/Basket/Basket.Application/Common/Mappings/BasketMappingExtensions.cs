using Basket.Contracts.DTOs;
using Basket.Domain.Entities;
using Basket.Domain.ValueObjects;
using BasketEntity = Basket.Domain.Entities.Basket;

namespace Basket.Application.Common.Mappings;

public static class BasketMappingExtensions
{
    public static BasketDto ToDto(
        this BasketEntity basket, 
        Dictionary<long, List<BasketItemWarningDto>>? itemWarnings = null)
    {
        var items = basket.Items.Select(i =>
        {
            List<BasketItemWarningDto>? warnings = null;
            if (itemWarnings != null && itemWarnings.TryGetValue(i.VariantId, out var w))
            {
                warnings = w;
            }

            return new BasketItemDto(
                i.ProductId,
                i.ProductName,
                i.VariantId,
                i.VariantName,
                i.Quantity,
                i.Price,
                i.DiscountPrice,
                i.DiscountPercent,
                i.FinalPrice,
                i.Promotion,
                i.Gifts,
                warnings
            );
        }).ToList();

        bool hasWarnings = items.Any(i => i.Warnings != null && i.Warnings.Count > 0);

        return new BasketDto(
            basket.Id,
            basket.CustomerId,
            basket.SessionId,
            basket.FulfillmentType.ToString(),
            new CustomerInfoDto(
                basket.CustomerInfo.Name,
                basket.CustomerInfo.Phone,
                basket.CustomerInfo.Email
            ),
            basket.ShippingInfo == null ? null : new ShippingInfoDto(
                basket.ShippingInfo.Address,
                basket.ShippingInfo.ProvinceId,
                basket.ShippingInfo.WardId,
                basket.ShippingInfo.Latitude,
                basket.ShippingInfo.Longitude
            ),
            basket.PaymentMethod,
            basket.ShippingMethod,
            items,
            basket.Subtotal,
            basket.Note,
            basket.ExpiresAt,
            hasWarnings
        );
    }
}
