using Catalog.Contracts.DTOs.Categories;
using MediatR;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Categories.Queries.GetCategoryDetailQuery;

public record GetCategoryDetailQuery(long Id) : IRequest<Result<CategoryDetailDto>>;
