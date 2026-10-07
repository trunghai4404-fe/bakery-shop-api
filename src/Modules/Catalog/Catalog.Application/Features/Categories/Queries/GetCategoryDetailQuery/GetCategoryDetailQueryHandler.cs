using Catalog.Contracts.DTOs.Categories;
using Catalog.Domain.Errors;
using Catalog.Domain.Repositories;
using MediatR;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Categories.Queries.GetCategoryDetailQuery;

public class GetCategoryDetailQueryHandler(
    ICategoryRepository categoryRepository
) : IRequestHandler<GetCategoryDetailQuery, Result<CategoryDetailDto>>
{
    public async Task<Result<CategoryDetailDto>> Handle(
        GetCategoryDetailQuery request,
        CancellationToken ct)
    {
        var category = await categoryRepository.GetByIdWithDetailsAsync(request.Id, ct);
        if (category is null)
        {
            return Result.Failure<CategoryDetailDto>(CatalogErrors.CategoryNotFound);
        }

        var childrenDtos = category.Children
            .OrderBy(c => c.SortOrder)
            .Select(c => new CategoriesListDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                ParentId = c.ParentId,
                ParentName = category.Name,
                ImageUrl = c.ImageUrl,
                SortOrder = c.SortOrder,
                IsFeatured = c.IsFeatured,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                ChildrenCount = c.Children?.Count ?? 0
            }).ToList();

        var response = new CategoryDetailDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ParentId = category.ParentId,
            ParentName = category.Parent?.Name,
            ImageUrl = category.ImageUrl,
            SortOrder = category.SortOrder,
            IsFeatured = category.IsFeatured,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            CreatedBy = category.CreatedBy,
            LastModifiedAt = category.LastModifiedAt,
            LastModifiedBy = category.LastModifiedBy,
            Children = childrenDtos
        };

        return Result.Success(response);
    }
}
