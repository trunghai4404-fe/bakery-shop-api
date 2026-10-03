using Catalog.Contracts.DTOs.Categories;
using MediatR;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Categories.Commands.CreateCategoryCommand;

public record CreateCategoryCommand(
    string Name,
    string? Slug = null,
    string? Description = null,
    long? ParentId = null,
    string? ImageUrl = null,
    bool IsActive = true,
    int SortOrder = 0
) : IRequest<Result<CreateCategoryResponse>>;
