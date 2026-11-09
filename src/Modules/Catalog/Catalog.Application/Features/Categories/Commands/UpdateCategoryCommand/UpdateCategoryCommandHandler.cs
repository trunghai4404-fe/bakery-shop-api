using Catalog.Contracts.DTOs.Categories;
using Catalog.Domain.Entities;
using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using MediatR;
using SharedKernel.Commons;
using SharedKernel.Domain;
using SharedKernel.Domain.Errors;

namespace Catalog.Application.Features.Categories.Commands.UpdateCategoryCommand;

public class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ICatalogUnitOfWork unitOfWork
) : IRequestHandler<UpdateCategoryCommand, Result<UpdateCategoryResponse>>
{
    public async Task<Result<UpdateCategoryResponse>> Handle(
        UpdateCategoryCommand request,
        CancellationToken ct)
    {
        // 1. Check if category exists
        var category = await categoryRepository.GetByIdAsync(request.Id, ct);
        if (category is null)
        {
            return Result.Failure<UpdateCategoryResponse>(CatalogErrors.CategoryNotFound);
        }

        // 2. Resolve & normalize slug
        var rawSlug = string.IsNullOrWhiteSpace(request.Slug) ? request.Name : request.Slug;
        var slug = SlugHelper.GenerateSlug(rawSlug);

        if (string.IsNullOrWhiteSpace(slug))
        {
            return Result.Failure<UpdateCategoryResponse>(
                Error.Validation(ErrorCode.ValidationError, "Category slug could not be generated. Please provide a valid name or slug."));
        }

        // 3. Validate slug uniqueness (excluding current category)
        var isSlugUnique = await categoryRepository.IsSlugUniqueAsync(slug, category.Id, ct);
        if (!isSlugUnique)
        {
            return Result.Failure<UpdateCategoryResponse>(CatalogErrors.CategorySlugAlreadyExists);
        }

        // 4. Validate parent category & check for circular hierarchy
        if (request.ParentId.HasValue)
        {
            if (request.ParentId.Value == category.Id)
            {
                return Result.Failure<UpdateCategoryResponse>(CatalogErrors.CircularCategoryParent);
            }

            var currentParentId = request.ParentId;
            while (currentParentId.HasValue)
            {
                if (currentParentId.Value == category.Id)
                {
                    return Result.Failure<UpdateCategoryResponse>(CatalogErrors.CircularCategoryParent);
                }

                var parentEntity = await categoryRepository.GetByIdAsync(currentParentId.Value, ct);
                if (parentEntity is null)
                {
                    return Result.Failure<UpdateCategoryResponse>(CatalogErrors.ParentCategoryNotFound);
                }

                currentParentId = parentEntity.ParentId;
            }
        }

        // 5. Update Category Contents.Domain Entity
        var updateResult = category.Update(
            name: request.Name.Trim(),
            slug: slug,
            description: request.Description?.Trim(),
            parentId: request.ParentId,
            imageUrl: request.ImageUrl?.Trim(),
            sortOrder: request.SortOrder,
            isFeatured: request.IsFeatured
        );

        if (updateResult.IsFailure)
        {
            return Result.Failure<UpdateCategoryResponse>(updateResult.Error);
        }

        // 6. Persist changes
        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync(ct);

        // 7. Map response
        var response = new UpdateCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ParentId = category.ParentId,
            ImageUrl = category.ImageUrl,
            IsFeatured = category.IsFeatured,
            IsActive = category.IsActive,
            SortOrder = category.SortOrder
        };

        return Result.Success(response);
    }
}
