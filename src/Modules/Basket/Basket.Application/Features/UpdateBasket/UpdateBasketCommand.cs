using Basket.Application.Common.Mappings;
using Basket.Contracts.DTOs;
using Basket.Domain.Entities;
using Basket.Domain.Enums;
using Basket.Domain.Errors;
using Basket.Domain.Repositories;
using Basket.Domain.ValueObjects;
using Catalog.Contracts.Interfaces;
using FluentValidation;
using MediatR;
using SharedKernel.Domain;

namespace Basket.Application.Features.UpdateBasket;

public record UpdateBasketCommand(
    Guid BasketId,
    UpdateBasketRequest Request
) : IRequest<Result<BasketDto>>;

public class UpdateBasketCommandValidator : AbstractValidator<UpdateBasketCommand>
{
    public UpdateBasketCommandValidator()
    {
        RuleFor(x => x.BasketId).NotEmpty();

        RuleFor(x => x.Request.FulfillmentType)
            .NotEmpty()
            .WithMessage("Hình thức nhận hàng không được để trống.")
            .Must(f => Enum.TryParse<FulfillmentType>(f, true, out var parsed) && Enum.IsDefined(typeof(FulfillmentType), parsed) && !int.TryParse(f, out _))
            .WithMessage("Hình thức nhận hàng không hợp lệ ('Pickup' hoặc 'Delivery').");

        When(x => string.Equals(x.Request.FulfillmentType, nameof(FulfillmentType.Delivery), StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(x => x.Request.ShippingInfo)
                .NotNull()
                .WithMessage("Hình thức giao hàng tận nơi bắt buộc phải có thông tin giao hàng.");

            RuleFor(x => x.Request.ShippingInfo!.Address)
                .NotEmpty()
                .When(x => x.Request.ShippingInfo != null)
                .WithMessage("Địa chỉ giao hàng không được để trống.");

            RuleFor(x => x.Request.ShippingInfo!.ProvinceId)
                .GreaterThan(0)
                .When(x => x.Request.ShippingInfo != null)
                .WithMessage("Mã Tỉnh/Thành phố không hợp lệ.");

            RuleFor(x => x.Request.ShippingInfo!.WardId)
                .GreaterThan(0)
                .When(x => x.Request.ShippingInfo != null)
                .WithMessage("Mã Phường/Xã không hợp lệ.");
        });
    }
}

public class UpdateBasketCommandHandler(
    IBasketRepository basketRepository,
    IBasketUnitOfWork unitOfWork,
    ICatalogContract catalogContract
) : IRequestHandler<UpdateBasketCommand, Result<BasketDto>>
{
    public async Task<Result<BasketDto>> Handle(UpdateBasketCommand command, CancellationToken ct)
    {
        var basket = await basketRepository.GetByIdAsync(command.BasketId, ct);
        if (basket == null)
        {
            return Result.Failure<BasketDto>(BasketErrors.NotFound);
        }

        var req = command.Request;

        // 1. Gom nhóm các item trùng lặp và loại bỏ item có quantity <= 0 (tương đương xóa item)
        var validItems = (req.Items ?? [])
            .Where(i => i.Quantity > 0)
            .GroupBy(i => (i.ProductId, i.VariantId))
            .Select(g => new BasketItemRequest(g.Key.ProductId, g.Key.VariantId, g.Sum(x => x.Quantity)))
            .ToList();

        var newItems = new List<BasketItem>();

        if (validItems.Count > 0)
        {
            var variantIds = validItems.Select(i => i.VariantId).ToList();
            var catalogVariants = await catalogContract.GetVariantsByIdsAsync(variantIds, ct);
            var catalogDict = catalogVariants.ToDictionary(v => v.VariantId);

            foreach (var item in validItems)
            {
                if (!catalogDict.TryGetValue(item.VariantId, out var catalogVar) || !catalogVar.IsActive)
                {
                    return Result.Failure<BasketDto>(BasketErrors.ProductNotFoundOrInactive(item.VariantId));
                }

                if (item.Quantity > catalogVar.StockQuantity)
                {
                    return Result.Failure<BasketDto>(BasketErrors.OutOfStock(catalogVar.ProductName, catalogVar.StockQuantity));
                }

                decimal price = catalogVar.Price;
                decimal discountPrice = 0;
                decimal discountPercent = 0;
                decimal finalPrice = price - discountPrice;

                newItems.Add(new BasketItem(
                    catalogVar.ProductId,
                    catalogVar.ProductName,
                    catalogVar.VariantId,
                    catalogVar.VariantName,
                    item.Quantity,
                    price,
                    discountPrice,
                    discountPercent,
                    finalPrice
                ));
            }
        }

        // 2. Cập nhật Fulfillment & Thông tin khách
        var customerInfo = req.CustomerInfo != null
            ? new CustomerInfo(req.CustomerInfo.Name, req.CustomerInfo.Phone, req.CustomerInfo.Email)
            : basket.CustomerInfo;

        var isDelivery = string.Equals(req.FulfillmentType, nameof(FulfillmentType.Delivery), StringComparison.OrdinalIgnoreCase);

        ShippingInfo? shippingInfo = null;
        if (isDelivery && req.ShippingInfo != null)
        {
            shippingInfo = new ShippingInfo(
                req.ShippingInfo.Address,
                req.ShippingInfo.ProvinceId,
                req.ShippingInfo.WardId,
                req.ShippingInfo.Latitude,
                req.ShippingInfo.Longitude,
                req.ShippingInfo.Note
            );
        }

        Enum.TryParse<FulfillmentType>(req.FulfillmentType, true, out var fulfillmentType);

        basket.UpdateItems(newItems);
        basket.UpdateFulfillment(fulfillmentType, customerInfo, shippingInfo, req.Note);

        basketRepository.Update(basket);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(basket.ToDto());
    }
}
