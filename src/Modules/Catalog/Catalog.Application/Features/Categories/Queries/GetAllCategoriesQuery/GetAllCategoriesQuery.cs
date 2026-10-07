using Catalog.Contracts.DTOs.Categories;
using MediatR;
using SharedKernel.Commons;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Categories.Queries.GetAllCategoriesQuery;

public record GetAllCategoriesQuery(
    string? Search = null,
    bool? IsActive = null,
    long? ParentId = null,
    bool? IsRoot = null,
    bool? IsFeatured = null,
    string? SortBy = null,
    bool IsDescending = false,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PagedList<CategoriesListDto>>>;
