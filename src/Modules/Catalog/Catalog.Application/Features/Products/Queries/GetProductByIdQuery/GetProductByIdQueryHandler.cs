using Catalog.Contracts.DTOs.Products;
using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using MediatR;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Products.Queries.GetProductByIdQuery;

public class GetProductByIdQueryHandler(
    IProductRepository productRepository
) : IRequestHandler<GetProductByIdQuery, Result<ProductDetailDto>>
{
    public async Task<Result<ProductDetailDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(request.Id, ct);
        if (product is null)
        {
            return Result.Failure<ProductDetailDto>(CatalogErrors.ProductNotFound);
        }

        var response = new ProductDetailDto
        {
            Id = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            Sku = product.Sku,
            Description = product.Description,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            CreatedBy = product.CreatedBy,
            LastModifiedAt = product.LastModifiedAt,
            LastModifiedBy = product.LastModifiedBy,
            Images = product.ProductImages
                .OrderBy(img => img.SortOrder)
                .Select(img => new ProductImageDto
                {
                    Id = img.Id,
                    ProductId = img.ProductId,
                    DesktopUrl = img.DesktopUrl,
                    MobileUrl = img.MobileUrl,
                    IsPrimary = img.IsPrimary,
                    SortOrder = img.SortOrder
                }).ToList(),
            Variants = product.ProductVariants
                .OrderBy(v => v.SortOrder)
                .Select(v => new ProductVariantDto
                {
                    Id = v.Id,
                    ProductId = v.ProductId,
                    Name = v.Name,
                    Sku = v.Sku,
                    Price = v.Price,
                    StockQuantity = v.StockQuantity,
                    IsActive = v.IsActive,
                    SortOrder = v.SortOrder
                }).ToList(),
            Categories = product.ProductCategories
                .Where(pc => pc.Category != null)
                .OrderBy(pc => pc.SortOrder)
                .Select(pc => new ProductCategoryDto
                {
                    CategoryId = pc.CategoryId,
                    Name = pc.Category!.Name,
                    Slug = pc.Category.Slug
                }).ToList(),
            Accessories = product.ProductAccessories
                .OrderBy(pa => pa.SortOrder)
                .Select(pa => new ProductAccessoryDto
                {
                    AccessoryProductId = pa.AccessoryProductId,
                    Name = pa.AccessoryProduct?.Name ?? string.Empty,
                    Slug = pa.AccessoryProduct?.Slug ?? string.Empty,
                    Sku = pa.AccessoryProduct?.Sku ?? string.Empty,
                    Price = pa.AccessoryProduct?.ProductVariants.Where(v => v.IsActive).Min(v => (decimal?)v.Price) 
                            ?? pa.AccessoryProduct?.ProductVariants.Min(v => (decimal?)v.Price),
                    PrimaryImageUrl = pa.AccessoryProduct?.ProductImages.FirstOrDefault(i => i.IsPrimary)?.DesktopUrl 
                                      ?? pa.AccessoryProduct?.ProductImages.OrderBy(i => i.SortOrder).FirstOrDefault()?.DesktopUrl,
                    DefaultQuantity = pa.DefaultQuantity,
                    IsIncluded = pa.IsIncluded,
                    SortOrder = pa.SortOrder,
                    IsMultiSelect = pa.IsMultiSelect,
                    MaxSelectCount = pa.MaxSelectCount,
                    IsRequired = pa.IsRequired,
                    Variants = pa.AccessoryProduct?.ProductVariants
                        .Where(v => v.IsActive)
                        .OrderBy(v => v.SortOrder)
                        .Select(v => new ProductVariantDto
                        {
                            Id = v.Id,
                            ProductId = v.ProductId,
                            Name = v.Name,
                            Sku = v.Sku,
                            Price = v.Price,
                            StockQuantity = v.StockQuantity,
                            IsActive = v.IsActive,
                            SortOrder = v.SortOrder
                        }).ToList() ?? new List<ProductVariantDto>()
                }).ToList()
        };

        return Result.Success(response);
    }
}
