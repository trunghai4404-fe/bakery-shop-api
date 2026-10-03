namespace Catalog.Contracts.DTOs.Products;

public record UpdateProductRequest(
    string Name,
    string Sku,
    string? Slug = null,
    string? Description = null,
    bool IsActive = true,
    List<UpdateProductImageRequest>? Images = null,
    List<UpdateProductVariantRequest>? Variants = null,
    List<long>? CategoryIds = null
);
