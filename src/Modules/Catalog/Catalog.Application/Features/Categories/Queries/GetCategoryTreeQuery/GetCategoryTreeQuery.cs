using Catalog.Contracts.DTOs.Categories;
using MediatR;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Categories.Queries.GetCategoryTreeQuery;

public record GetCategoryTreeQuery(bool? OnlyActive = true) : IRequest<Result<List<CategoryTreeDto>>>;
