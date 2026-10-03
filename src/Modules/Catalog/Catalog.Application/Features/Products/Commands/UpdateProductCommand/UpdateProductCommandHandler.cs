using Catalog.Contracts.DTOs.Products;
using Catalog.Domain.Entities;
using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using MediatR;
using SharedKernel.Commons;
using SharedKernel.Domain;
using SharedKernel.Domain.Errors;

namespace Catalog.Application.Features.Products.Commands.UpdateProductCommand;

public class UpdateProductCommandHandler(
    IProductRepository productRepository,
    ICatalogUnitOfWork unitOfWork
) : IRequestHandler<UpdateProductCommand, Result<UpdateProductResponse>>
{
    public async Task<Result<UpdateProductResponse>> Handle(
        UpdateProductCommand request,
        CancellationToken ct)
    {
        // 1. Check if product exists
        var product = await productRepository.GetByIdForUpdateAsync(request.Id, ct);
        if (product is null)
        {
            return Result.Failure<UpdateProductResponse>(CatalogErrors.ProductNotFound);
        }

        // 2. Resolve & normalize slug
        var rawSlug = string.IsNullOrWhiteSpace(request.Slug) ? request.Name : request.Slug;
        var slug = SlugHelper.GenerateSlug(rawSlug);

        if (string.IsNullOrWhiteSpace(slug))
        {
            return Result.Failure<UpdateProductResponse>(
                Error.Validation(ErrorCode.ValidationError, "Product slug could not be generated. Please provide a valid name or slug."));
        }

        // 3. Validate slug uniqueness (excluding current product)
        var isSlugUnique = await productRepository.IsSlugUniqueAsync(slug, product.Id, ct);
        if (!isSlugUnique)
        {
            return Result.Failure<UpdateProductResponse>(CatalogErrors.ProductSlugAlreadyExists);
        }

        // 4. Validate SKU uniqueness (excluding current product)
        var isSkuUnique = await productRepository.IsSkuUniqueAsync(request.Sku.Trim(), product.Id, ct);
        if (!isSkuUnique)
        {
            return Result.Failure<UpdateProductResponse>(CatalogErrors.ProductSkuAlreadyExists);
        }

        // 5. Update Product entity
        var updateResult = product.Update(
            name: request.Name.Trim(),
            slug: slug,
            sku: request.Sku.Trim(),
            description: request.Description?.Trim(),
            isActive: request.IsActive
        );

        if (updateResult.IsFailure)
        {
            return Result.Failure<UpdateProductResponse>(updateResult.Error);
        }

        // 6. Sync Images if provided
        if (request.Images != null)
        {
            var imageDatas = request.Images.Select(i => new Product.ImageData(
                i.Id,
                i.DesktopUrl.Trim(),
                i.MobileUrl.Trim(),
                i.SortOrder,
                i.IsPrimary
            )).ToList();

            product.SyncImages(imageDatas);
        }

        // 7. Sync Variants if provided
        if (request.Variants != null)
        {
            var variantDatas = request.Variants.Select(v => new Product.VariantData(
                v.Id,
                v.Name.Trim(),
                v.Sku.Trim(),
                v.Price,
                v.StockQuantity,
                v.IsActive,
                v.SortOrder
            )).ToList();

            product.SyncVariants(variantDatas);
        }

        // 8. Sync Categories if provided
        if (request.CategoryIds != null)
        {
            product.SyncCategories(request.CategoryIds);
        }

        // 9. Persist changes
        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync(ct);

        // 10. Map response
        var response = new UpdateProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            Sku = product.Sku,
            Description = product.Description,
            IsActive = product.IsActive
        };

        return Result.Success(response);
    }
}
