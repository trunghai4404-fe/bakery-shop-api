using Ordering.Contracts.DTOs;
using Ordering.Domain.Entities;
using Ordering.Domain.Enums;

namespace Ordering.Application.Common.Mappings;

public static class OrderMappingExtensions
{
    public static OrderDto ToOrderDto(this Order order)
    {
        return new OrderDto(
            order.Id,
            order.OrderCode,
            order.UserId,
            order.SessionId,
            order.FulfillmentType,
            new OrderCustomerInfoDto(
                order.CustomerInfo.Name,
                order.CustomerInfo.Phone,
                order.CustomerInfo.Email
            ),
            order.ShippingAddress == null ? null : new OrderShippingAddressDto(
                order.ShippingAddress.Address,
                order.ShippingAddress.ProvinceId,
                order.ShippingAddress.WardId,
                order.ShippingAddress.Latitude,
                order.ShippingAddress.Longitude
            ),
            order.ShippingMethodCode,
            order.ShippingMethodName,
            order.ShippingFee,
            order.ShippingFeeNote,
            order.PaymentMethodCode,
            order.PaymentMethodName,
            order.PaymentStatus.ToString(),
            order.Status.ToString(),
            order.Subtotal,
            order.DiscountAmount,
            order.TotalAmount,
            order.Note,
            order.CancelReason,
            order.CancelledAt,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemDto(
                i.Id,
                i.ProductId,
                i.ProductName,
                i.VariantId,
                i.VariantName,
                i.ProductImageUrl,
                i.Quantity,
                i.Price,
                i.DiscountPrice,
                i.DiscountPercent,
                i.FinalPrice,
                i.TotalItemPrice
            )).ToList(),
            order.StoreInfo == null ? null : new OrderStoreDto(
                order.StoreInfo.Id,
                order.StoreInfo.Name,
                order.StoreInfo.Address,
                order.StoreInfo.ProvinceId,
                order.StoreInfo.WardId,
                order.StoreInfo.Latitude,
                order.StoreInfo.Longitude,
                order.StoreInfo.Phone,
                order.StoreInfo.Email,
                order.StoreInfo.OpeningHours
            )
        );
    }

    public static OrderDetailDto ToOrderDetailDto(this Order order)
    {
        return new OrderDetailDto(order.ToOrderDto(), MapStatusHistories(order));
    }

    public static UserOrderSummaryDto ToUserSummaryDto(this Order order)
    {
        return new UserOrderSummaryDto(
            order.OrderCode,
            order.CreatedAt,
            order.Status.ToString(),
            order.PaymentStatus.ToString(),
            order.PaymentMethodCode,
            order.PaymentMethodName,
            order.ShippingMethodCode,
            order.ShippingMethodName,
            order.FulfillmentType,
            order.TotalAmount,
            order.Items.Sum(i => i.Quantity),
            order.Items.Select(i => new UserOrderItemSummaryDto(
                i.ProductId,
                i.ProductName,
                i.VariantId,
                i.VariantName,
                i.ProductImageUrl,
                i.Quantity,
                i.FinalPrice,
                i.TotalItemPrice
            )).ToList(),
            order.StoreInfo == null ? null : new OrderStoreDto(
                order.StoreInfo.Id,
                order.StoreInfo.Name,
                order.StoreInfo.Address,
                order.StoreInfo.ProvinceId,
                order.StoreInfo.WardId,
                order.StoreInfo.Latitude,
                order.StoreInfo.Longitude,
                order.StoreInfo.Phone,
                order.StoreInfo.Email,
                order.StoreInfo.OpeningHours
            )
        );
    }

    public static PublicOrderLookupDto ToPublicLookupDto(this Order order)
    {
        return new PublicOrderLookupDto(
            order.OrderCode,
            order.Status.ToString(),
            order.PaymentStatus.ToString(),
            order.PaymentMethodCode,
            order.PaymentMethodName,
            order.ShippingMethodCode,
            order.ShippingMethodName,
            order.FulfillmentType,
            MaskName(order.CustomerInfo.Name),
            MaskPhone(order.CustomerInfo.Phone),
            order.ShippingAddress != null ? MaskAddress(order.ShippingAddress.Address) : null,
            order.Subtotal,
            order.ShippingFee,
            order.TotalAmount,
            order.Note,
            order.CancelReason,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemDto(
                i.Id,
                i.ProductId,
                i.ProductName,
                i.VariantId,
                i.VariantName,
                i.ProductImageUrl,
                i.Quantity,
                i.Price,
                i.DiscountPrice,
                i.DiscountPercent,
                i.FinalPrice,
                i.TotalItemPrice
            )).ToList(),
            order.StoreInfo == null ? null : new OrderStoreDto(
                order.StoreInfo.Id,
                order.StoreInfo.Name,
                order.StoreInfo.Address,
                order.StoreInfo.ProvinceId,
                order.StoreInfo.WardId,
                order.StoreInfo.Latitude,
                order.StoreInfo.Longitude,
                order.StoreInfo.Phone,
                order.StoreInfo.Email,
                order.StoreInfo.OpeningHours
            )
        )
        {
            Histories = MapStatusHistories(order)
        };
    }

    private static List<OrderStatusHistoryDto>? MapStatusHistories(Order order)
    {
        var histories = order.StatusHistories.OrderBy(history => history.Id).ToList();
        if (histories.Count == 0 ||
            !histories[0].IsOrderCreation ||
            histories[0].Status != OrderStatus.Pending ||
            histories[0].CreatedTime != order.CreatedAt ||
            histories[^1].Status != order.Status)
        {
            return null;
        }

        return histories
            .Select(history => new OrderStatusHistoryDto(history.Status.ToString(), history.CreatedTime))
            .ToList();
    }

    private static string MaskPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone) || phone.Length < 7)
            return "***";

        var prefix = phone[..4];
        var suffix = phone[^3..];
        return $"{prefix}***{suffix}";
    }

    private static string MaskName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "***";

        var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length <= 1)
        {
            return name.Length <= 2 ? name : $"{name[0]}***{name[^1]}";
        }

        // e.g. "Nguyễn Văn A" -> "Nguyễn V** A"
        for (int i = 1; i < parts.Length - 1; i++)
        {
            parts[i] = $"{parts[i][0]}**";
        }
        return string.Join(" ", parts);
    }

    private static string MaskAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return "***";

        var parts = address.Split(',', StringSplitOptions.TrimEntries);
        if (parts.Length > 1)
        {
            // Ẩn số nhà/đường đầu tiên, giữ lại quận huyện tỉnh thành
            parts[0] = "***";
            return string.Join(", ", parts);
        }

        return address.Length > 10 ? $"*** {address[^10..]}" : "***";
    }
}
