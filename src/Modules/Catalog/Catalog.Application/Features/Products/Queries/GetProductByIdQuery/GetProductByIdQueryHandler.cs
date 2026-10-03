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
                }).ToList()
        };

        return Result.Success(response);
    }
}
