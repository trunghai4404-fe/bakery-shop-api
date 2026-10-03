using Catalog.Contracts.DTOs.Products;
using MediatR;
using SharedKernel.Commons;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Products.Queries.GetProductsQuery;

public record GetProductsQuery(
    string? Search = null,
    long? CategoryId = null,
    string? CategorySlug = null,
    bool? IsActive = null,
    string? SortBy = null,
    bool IsDescending = false,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PagedList<ProductListDto>>>;
