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
using BasketEntity = Basket.Domain.Entities.Basket;

namespace Basket.Application.Features.CreateBasket;

public record CreateBasketCommand(
    long? CustomerId,
    string? SessionId,
    CreateBasketRequest Request
) : IRequest<Result<BasketDto>>;

public class CreateBasketCommandValidator : AbstractValidator<CreateBasketCommand>
{
    public CreateBasketCommandValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.Request.FulfillmentType), () =>
        {
            RuleFor(x => x.Request.FulfillmentType!)
                .Must(f => Enum.TryParse<FulfillmentType>(f, true, out var parsed) && Enum.IsDefined(typeof(FulfillmentType), parsed) && !int.TryParse(f, out _))
                .WithMessage("Hình thức nhận hàng không hợp lệ ('Pickup' hoặc 'Delivery').");
        });
    }
}

public class CreateBasketCommandHandler(
    IBasketRepository basketRepository,
    IBasketUnitOfWork unitOfWork,
    ICatalogContract catalogContract
) : IRequestHandler<CreateBasketCommand, Result<BasketDto>>
{
    public async Task<Result<BasketDto>> Handle(CreateBasketCommand command, CancellationToken ct)
    {
        // 1. Kiểm tra xem Customer hoặc Session này đã có Active Basket chưa
        BasketEntity? existingBasket = null;
        if (command.CustomerId.HasValue)
        {
            existingBasket = await basketRepository.GetActiveByCustomerIdAsync(command.CustomerId.Value, ct);
        }
        else if (!string.IsNullOrWhiteSpace(command.SessionId))
        {
            existingBasket = await basketRepository.GetActiveBySessionIdAsync(command.SessionId, ct);
        }

        // 2. Nếu đã có Active Basket, tái sử dụng giỏ này
        var basket = existingBasket ?? BasketEntity.Create(command.CustomerId, command.SessionId);

        // 3. Nếu request có items gửi kèm
        if (command.Request.Items != null && command.Request.Items.Count > 0)
        {
            var validItems = command.Request.Items
                .Where(i => i.Quantity > 0)
                .GroupBy(i => (i.ProductId, i.VariantId))
                .Select(g => new BasketItemRequest(g.Key.ProductId, g.Key.VariantId, g.Sum(x => x.Quantity)))
                .ToList();

            var variantIds = validItems.Select(i => i.VariantId).ToList();
            var catalogVariants = await catalogContract.GetVariantsByIdsAsync(variantIds, ct);
            var catalogDict = catalogVariants.ToDictionary(v => v.VariantId);

            var newItems = new List<BasketItem>();
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
                decimal discountPrice = 0; // Tích hợp với Promotion Catalog nếu có
                decimal discountPercent = price > 0 ? (discountPrice / price) * 100 : 0;
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

            basket.UpdateItems(newItems);
        }

        if (!string.IsNullOrWhiteSpace(command.Request.FulfillmentType) &&
            Enum.TryParse<FulfillmentType>(command.Request.FulfillmentType, true, out var fType))
        {
            basket.UpdateFulfillment(fType, basket.CustomerInfo, basket.ShippingInfo, command.Request.Note ?? basket.Note);
        }

        if (existingBasket == null)
        {
            await basketRepository.AddAsync(basket, ct);
        }
        else
        {
            basketRepository.Update(basket);
        }

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(basket.ToDto());
    }
}
