using Catalog.Contracts.DTOs.Categories;
using MediatR;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Categories.Commands.ToggleCategoryCommand;

public record ToggleCategoryCommand(
    long Id,
    bool? IsActive = null
) : IRequest<Result<ToggleCategoryResponse>>;
