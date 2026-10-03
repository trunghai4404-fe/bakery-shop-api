using Catalog.Contracts.DTOs.Products;
using Catalog.Domain.Entities;
using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using MediatR;
using SharedKernel.Commons;
using SharedKernel.Domain;
using SharedKernel.Domain.Errors;

namespace Catalog.Application.Features.Products.Commands.CreateProductCommand;

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    ICatalogUnitOfWork unitOfWork
) : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    public async Task<Result<CreateProductResponse>> Handle(
        CreateProductCommand request,
        CancellationToken ct)
    {
        // 1. Generate slug
        var rawSlug = string.IsNullOrWhiteSpace(request.Slug) ? request.Name : request.Slug;
        var slug = SlugHelper.GenerateSlug(rawSlug);

        if (string.IsNullOrWhiteSpace(slug))
        {
            return Result.Failure<CreateProductResponse>(
                Error.Validation(ErrorCode.ValidationError, "Product slug could not be generated. Please provide a valid name or slug."));
        }

        // 2. Validate slug uniqueness
        var isSlugUnique = await productRepository.IsSlugUniqueAsync(slug, null, ct);
        if (!isSlugUnique)
        {
            return Result.Failure<CreateProductResponse>(CatalogErrors.ProductSlugAlreadyExists);
        }

        // 3. Validate SKU uniqueness
        var isSkuUnique = await productRepository.IsSkuUniqueAsync(request.Sku.Trim(), null, ct);
        if (!isSkuUnique)
        {
            return Result.Failure<CreateProductResponse>(CatalogErrors.ProductSkuAlreadyExists);
        }

        // 4. Create Product entity
        var productResult = Product.Create(
            name: request.Name.Trim(),
            slug: slug,
            sku: request.Sku.Trim(),
            description: request.Description?.Trim(),
            isActive: request.IsActive
        );

        if (productResult.IsFailure)
        {
            return Result.Failure<CreateProductResponse>(productResult.Error);
        }

        var product = productResult.Value;

        // 5. Add Images if provided
        if (request.Images != null && request.Images.Count > 0)
        {
            foreach (var img in request.Images)
            {
                product.AddImage(
                    desktopUrl: img.DesktopUrl.Trim(),
                    mobileUrl: img.MobileUrl.Trim(),
                    sortOrder: img.SortOrder,
                    isPrimary: img.IsPrimary
                );
            }
        }

        // 6. Add Variants if provided
        if (request.Variants != null && request.Variants.Count > 0)
        {
            foreach (var v in request.Variants)
            {
                product.AddVariant(
                    name: v.Name.Trim(),
                    sku: v.Sku.Trim(),
                    price: v.Price,
                    stockQuantity: v.StockQuantity
                );
            }
        }

        // 7. Add Categories if provided
        if (request.CategoryIds != null && request.CategoryIds.Count > 0)
        {
            foreach (var categoryId in request.CategoryIds.Distinct())
            {
                product.AddCategory(categoryId);
            }
        }

        // 8. Persist entity
        await productRepository.AddAsync(product, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // 9. Map to response
        var response = new CreateProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            Sku = product.Sku,
            Description = product.Description,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt
        };

        return Result.Success(response);
    }
}
