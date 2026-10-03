namespace Catalog.Contracts.DTOs.Products;

public record CreateProductVariantRequest(
    string Name,
    string Sku,
    decimal Price,
    int StockQuantity
);
