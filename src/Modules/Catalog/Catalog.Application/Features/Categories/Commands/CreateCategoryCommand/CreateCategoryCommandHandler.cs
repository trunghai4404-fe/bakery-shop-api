using Catalog.Contracts.DTOs.Categories;
using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using MediatR;
using SharedKernel.Commons;
using SharedKernel.Domain;
using SharedKernel.Domain.Errors;
using Catalog.Domain.Entities;

namespace Catalog.Application.Features.Categories.Commands.CreateCategoryCommand;

public class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ICatalogUnitOfWork unitOfWork
) : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryResponse>>
{
    public async Task<Result<CreateCategoryResponse>> Handle(
        CreateCategoryCommand request,
        CancellationToken ct)
    {
        var rawSlug = string.IsNullOrWhiteSpace(request.Slug) ? request.Name : request.Slug;
        var slug = SlugHelper.GenerateSlug(rawSlug);

        if (string.IsNullOrWhiteSpace(slug))
        {
            return Result.Failure<CreateCategoryResponse>(
                Error.Validation(ErrorCode.ValidationError, "Category slug could not be generated. Please provide a valid name or slug."));
        }

        // 2. Validate slug uniqueness
        var isSlugUnique = await categoryRepository.IsSlugUniqueAsync(slug,null, ct);
        if (!isSlugUnique)
        {
            return Result.Failure<CreateCategoryResponse>(CatalogErrors.CategorySlugAlreadyExists);
        }

        // 3. Validate parent category existence if specified
        if (request.ParentId.HasValue)
        {
            var parent = await categoryRepository.GetByIdAsync(request.ParentId.Value, ct);
            if (parent is null)
            {
                return Result.Failure<CreateCategoryResponse>(CatalogErrors.ParentCategoryNotFound);
            }
        }

        // 4. Create Category entity via Domain factory
        var categoryResult = Category.Create(
            name: request.Name.Trim(),
            slug: slug,
            description: request.Description?.Trim(),
            parentId: request.ParentId,
            imageUrl: request.ImageUrl?.Trim(),
            isActive: request.IsActive,
            sortOrder: request.SortOrder
        );

        if (categoryResult.IsFailure)
        {
            return Result.Failure<CreateCategoryResponse>(categoryResult.Error);
        }

        var category = categoryResult.Value;

        // 5. Persist entity
        await categoryRepository.AddAsync(category, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // 6. Map to response
        var response = new CreateCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ParentId = category.ParentId,
            ImageUrl = category.ImageUrl,
            IsActive = category.IsActive,
            SortOrder = category.SortOrder
        };

        return Result.Success(response);
    }
}
