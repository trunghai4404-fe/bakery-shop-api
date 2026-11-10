using Basket.Application.Common.Mappings;
using Basket.Contracts.DTOs;
using Basket.Domain.Errors;
using Basket.Domain.Repositories;
using Basket.Domain.ValueObjects;
using Catalog.Contracts.Interfaces;
using MediatR;
using SharedKernel.Domain;

namespace Basket.Application.Features.GetBasketById;

public record GetBasketByIdQuery(Guid BasketId) : IRequest<Result<BasketDto>>;

public class GetBasketByIdQueryHandler(
    IBasketRepository basketRepository,
    IBasketUnitOfWork unitOfWork,
    ICatalogContract catalogContract
) : IRequestHandler<GetBasketByIdQuery, Result<BasketDto>>
{
    public async Task<Result<BasketDto>> Handle(GetBasketByIdQuery query, CancellationToken ct)
    {
        var basket = await basketRepository.GetByIdAsync(query.BasketId, ct);
        if (basket == null)
        {
            return Result.Failure<BasketDto>(BasketErrors.NotFound);
        }

        var itemWarnings = new Dictionary<long, List<BasketItemWarningDto>>();
        void AddWarning(long variantId, string code, string message)
        {
            if (!itemWarnings.TryGetValue(variantId, out var list))
            {
                list = [];
                itemWarnings[variantId] = list;
            }
            list.Add(new BasketItemWarningDto(code, message));
        }

        // Revalidate nhanh với Catalog nếu giỏ hàng có items
        if (basket.Items.Count > 0)
        {
            var variantIds = basket.Items.Select(i => i.VariantId).Distinct().ToList();
            var catalogVariants = await catalogContract.GetVariantsByIdsAsync(variantIds, ct);
            var catalogDict = catalogVariants.ToDictionary(v => v.VariantId);

            var updatedItems = new List<BasketItem>();
            bool hasPriceChanges = false;

            foreach (var item in basket.Items)
            {
                if (!catalogDict.TryGetValue(item.VariantId, out var catalogVar) || !catalogVar.IsActive)
                {
                    AddWarning(item.VariantId, "INACTIVE", $"Sản phẩm '{item.ProductName}' ({item.VariantName}) hiện không còn mở bán.");
                    updatedItems.Add(item);
                    continue;
                }

                if (item.Quantity > catalogVar.StockQuantity)
                {
                    AddWarning(item.VariantId, "OUT_OF_STOCK", $"Sản phẩm '{item.ProductName}' ({item.VariantName}) chỉ còn {catalogVar.StockQuantity} cái trong kho (bạn đang chọn {item.Quantity}).");
                }

                if (item.Price != catalogVar.Price)
                {
                    AddWarning(item.VariantId, "PRICE_CHANGED", $"Giá của '{item.ProductName}' ({item.VariantName}) đã thay đổi từ {item.Price:N0}đ thành {catalogVar.Price:N0}đ.");
                    hasPriceChanges = true;

                    decimal newPrice = catalogVar.Price;
                    decimal newDiscountPrice = 0;
                    decimal newDiscountPercent = 0;
                    decimal newFinalPrice = newPrice - newDiscountPrice;

                    updatedItems.Add(item with
                    {
                        Price = newPrice,
                        DiscountPrice = newDiscountPrice,
                        DiscountPercent = newDiscountPercent,
                        FinalPrice = newFinalPrice
                    });
                }
                else
                {
                    updatedItems.Add(item);
                }
            }

            if (hasPriceChanges)
            {
                basket.UpdateItems(updatedItems);
                basketRepository.Update(basket);
                await unitOfWork.SaveChangesAsync(ct);
            }
        }

        return Result.Success(basket.ToDto(itemWarnings.Count > 0 ? itemWarnings : null));
    }
}
