using Catalog.Contracts.DTOs.Products;
using MediatR;
using SharedKernel.Domain;

namespace Catalog.Application.Features.Products.Commands.UpdateProductCommand;

public record UpdateProductCommand(
    long Id,
    string Name,
    string Sku,
    string? Slug = null,
    string? Description = null,
    bool IsActive = true,
    List<UpdateProductImageRequest>? Images = null,
    List<UpdateProductVariantRequest>? Variants = null,
    List<long>? CategoryIds = null
) : IRequest<Result<UpdateProductResponse>>;
