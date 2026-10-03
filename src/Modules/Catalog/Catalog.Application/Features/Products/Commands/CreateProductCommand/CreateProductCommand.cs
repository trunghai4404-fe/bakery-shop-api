using Catalog.Contracts.DTOs.Products;
using MediatR;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Products.Commands.CreateProductCommand;

public record CreateProductCommand(
    string Name,
    string Sku,
    string? Slug = null,
    string? Description = null,
    bool IsActive = true,
    List<CreateProductImageRequest>? Images = null,
    List<CreateProductVariantRequest>? Variants = null,
    List<long>? CategoryIds = null
) : IRequest<Result<CreateProductResponse>>;
