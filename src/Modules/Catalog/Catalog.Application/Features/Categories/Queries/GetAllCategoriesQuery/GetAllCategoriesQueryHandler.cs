using Catalog.Contracts.DTOs.Categories;
using Catalog.Domain.Repositories;
using MediatR;
using SharedKernel.Commons;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Categories.Queries.GetAllCategoriesQuery;

public class GetAllCategoriesQueryHandler(
    ICategoryRepository categoryRepository
) : IRequestHandler<GetAllCategoriesQuery, Result<PagedList<CategoriesListDto>>>
{
    public async Task<Result<PagedList<CategoriesListDto>>> Handle(
        GetAllCategoriesQuery request,
        CancellationToken ct)
    {
        var pagedCategories = await categoryRepository.GetCategoriesAsync(
            request.Search,
            request.IsActive,
            request.ParentId,
            request.IsRoot,
            request.IsFeatured,
            request.SortBy,
            request.IsDescending,
            request.Page,
            request.PageSize,
            ct
        );

        var dtos = pagedCategories.Items.Select(c => new CategoriesListDto
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            Description = c.Description,
            ParentId = c.ParentId,
            ParentName = c.Parent?.Name,
            ImageUrl = c.ImageUrl,
            SortOrder = c.SortOrder,
            IsFeatured = c.IsFeatured,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            ChildrenCount = c.Children?.Count ?? 0
        }).ToList();

        var response = new PagedList<CategoriesListDto>(
            dtos,
            pagedCategories.Page,
            pagedCategories.PageSize,
            pagedCategories.TotalCount
        );

        return Result.Success(response);
    }
}
