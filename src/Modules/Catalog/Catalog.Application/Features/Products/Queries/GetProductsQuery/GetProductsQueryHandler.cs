using Catalog.Contracts.DTOs.Products;
using Catalog.Domain.Repositories;
using MediatR;
using SharedKernel.Commons;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Products.Queries.GetProductsQuery;

public class GetProductsQueryHandler(
    IProductRepository productRepository
) : IRequestHandler<GetProductsQuery, Result<PagedList<ProductListDto>>>
{
    public async Task<Result<PagedList<ProductListDto>>> Handle(
        GetProductsQuery request,
        CancellationToken ct)
    {
        var pagedProducts = await productRepository.GetProductsAsync(
            request.Search,
            request.CategoryId,
            request.CategorySlug,
            request.IsActive,
            request.SortBy,
            request.IsDescending,
            request.Page,
            request.PageSize,
            ct
        );

        var dtos = pagedProducts.Items.Select(p => new ProductListDto
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug,
            Sku = p.Sku,
            Description = p.Description,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt,
            Images = p.ProductImages
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
            Variants = p.ProductVariants
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
            Categories = p.ProductCategories
                .Where(pc => pc.Category != null)
                .OrderBy(pc => pc.SortOrder)
                .Select(pc => new ProductCategoryDto
                {
                    CategoryId = pc.CategoryId,
                    Name = pc.Category!.Name,
                    Slug = pc.Category.Slug
                }).ToList()
        }).ToList();

        var response = new PagedList<ProductListDto>(
            dtos,
            pagedProducts.Page,
            pagedProducts.PageSize,
            pagedProducts.TotalCount
        );

        return Result.Success(response);
    }
}
