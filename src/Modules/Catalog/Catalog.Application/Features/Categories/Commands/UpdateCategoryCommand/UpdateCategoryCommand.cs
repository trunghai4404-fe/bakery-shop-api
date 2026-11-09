using Catalog.Contracts.DTOs.Categories;
using MediatR;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Categories.Commands.UpdateCategoryCommand;

public record UpdateCategoryCommand(
    long Id,
    string Name,
    string? Slug = null,
    string? Description = null,
    long? ParentId = null,
    string? ImageUrl = null,
    int SortOrder = 0,
    bool IsFeatured = false
) : IRequest<Result<UpdateCategoryResponse>>;
