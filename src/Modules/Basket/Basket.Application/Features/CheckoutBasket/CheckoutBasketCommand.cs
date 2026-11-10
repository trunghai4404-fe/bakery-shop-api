using Basket.Contracts.DTOs;
using Basket.Domain.Enums;
using Basket.Domain.Errors;
using Basket.Domain.Repositories;
using Catalog.Contracts.Interfaces;
using MediatR;
using SharedKernel.Domain;

namespace Basket.Application.Features.CheckoutBasket;

public record CheckoutBasketCommand(
    Guid BasketId,
    CheckoutBasketRequest Request
) : IRequest<Result<CheckoutResultDto>>;

public class CheckoutBasketCommandHandler(
    IBasketRepository basketRepository,
    IBasketUnitOfWork unitOfWork,
    ICatalogContract catalogContract
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

        // Tái kiểm tra Catalog lần cuối trước khi chốt đơn
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

        // Snapshot tạo đơn hàng (Sẽ kết nối Order Module)
        var orderId = Guid.NewGuid();
        var orderCode = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
        var totalAmount = basket.Subtotal;

        // Chỉ xóa giỏ hàng KHI đơn hàng được khởi tạo thành công
        basket.Clear();
        basketRepository.Update(basket);
        await unitOfWork.SaveChangesAsync(ct);

        var result = new CheckoutResultDto(
            orderId,
            orderCode,
            totalAmount,
            "Pending"
        );

        return Result.Success(result);
    }
}
