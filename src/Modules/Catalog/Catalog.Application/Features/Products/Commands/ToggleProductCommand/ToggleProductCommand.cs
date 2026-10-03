using Catalog.Contracts.DTOs.Products;
using MediatR;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Products.Commands.ToggleProductCommand;

public record ToggleProductCommand(
    long Id,
    bool? IsActive = null
) : IRequest<Result<ToggleProductResponse>>;
