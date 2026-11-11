using Basket.Contracts.DTOs;
using Basket.Domain.Enums;
using Basket.Domain.Errors;
using Basket.Domain.Repositories;
using Catalog.Contracts.Interfaces;
using MediatR;
using Ordering.Contracts.DTOs;
using Ordering.Contracts.Interfaces;
using SharedKernel.Domain;

namespace Basket.Application.Features.CheckoutBasket;

public record CheckoutBasketCommand(Guid BasketId) : IRequest<Result<CheckoutResultDto>>;

public class CheckoutBasketCommandHandler(
    IBasketRepository basketRepository,
    IBasketUnitOfWork unitOfWork,
    ICatalogContract catalogContract,
    IOrderingContract orderingContract
) : IRequestHandler<CheckoutBasketCommand, Result<CheckoutResultDto>>
{
    public async Task<Result<CheckoutResultDto>> Handle(CheckoutBasketCommand command, CancellationToken ct)
    {
        var basket = await basketRepository.GetByIdAsync(command.BasketId, ct);
        if (basket == null)
        {
            return Result.Failure<CheckoutResultDto>(BasketErrors.NotFound);
        }

        if (basket.Items.Count == 0)
        {
            return Result.Failure<CheckoutResultDto>(BasketErrors.Empty);
        }

        if (basket.FulfillmentType == FulfillmentType.Delivery && basket.ShippingInfo == null)
        {
            return Result.Failure<CheckoutResultDto>(BasketErrors.ShippingInfoRequired);
        }

        // 1. Tái kiểm tra Catalog lần cuối trước khi chốt đơn
        var variantIds = basket.Items.Select(i => i.VariantId).Distinct().ToList();
        var catalogVariants = await catalogContract.GetVariantsByIdsAsync(variantIds, ct);
        var catalogDict = catalogVariants.ToDictionary(v => v.VariantId);

        foreach (var item in basket.Items)
        {
            if (!catalogDict.TryGetValue(item.VariantId, out var catalogVar) || !catalogVar.IsActive)
            {
                return Result.Failure<CheckoutResultDto>(BasketErrors.ProductNotFoundOrInactive(item.VariantId));
            }

            if (item.Quantity > catalogVar.StockQuantity)
            {
                return Result.Failure<CheckoutResultDto>(BasketErrors.OutOfStock(catalogVar.ProductName, catalogVar.StockQuantity));
            }
        }

        // 2. Chuyển đổi dữ liệu từ chính giỏ hàng và gọi sang Ordering Module để tạo Đơn hàng thực tế
        var orderItems = basket.Items.Select(i => new CreateOrderItemRequest(
            i.ProductId,
            i.ProductName,
            i.VariantId,
            i.VariantName,
            i.Quantity,
            i.Price,
            i.DiscountPrice,
            i.DiscountPercent,
            i.FinalPrice,
            i.ImageUrl
        )).ToList();

        var customerInfo = new OrderCustomerInfoDto(
            basket.CustomerInfo.Name,
            basket.CustomerInfo.Phone,
            basket.CustomerInfo.Email
        );

        OrderShippingAddressDto? shippingAddress = null;
        if (basket.FulfillmentType == FulfillmentType.Delivery && basket.ShippingInfo != null)
        {
            shippingAddress = new OrderShippingAddressDto(
                basket.ShippingInfo.Address,
                basket.ShippingInfo.ProvinceId,
                basket.ShippingInfo.WardId,
                basket.ShippingInfo.Latitude,
                basket.ShippingInfo.Longitude
            );
        }

        var createOrderRequest = new CreateOrderFromBasketRequest(
            basket.UserId,
            basket.SessionId,
            basket.FulfillmentType.ToString(),
            customerInfo,
            shippingAddress,
            basket.PaymentMethod,
            basket.ShippingMethod,
            orderItems,
            basket.Note
        );

        var orderResult = await orderingContract.CreateOrderFromBasketAsync(createOrderRequest, ct);
        if (!orderResult.IsSuccess)
        {
            return Result.Failure<CheckoutResultDto>(orderResult.Error);
        }

        // 3. Chỉ xóa sạch giỏ hàng KHI đơn hàng được khởi tạo thành công vào Database
        basket.Clear();
        basketRepository.Update(basket);
        await unitOfWork.SaveChangesAsync(ct);

        var result = new CheckoutResultDto(
            orderResult.Value.OrderCode,
            orderResult.Value.TotalAmount,
            orderResult.Value.PaymentStatus
        );

        return Result.Success(result);
    }
}
