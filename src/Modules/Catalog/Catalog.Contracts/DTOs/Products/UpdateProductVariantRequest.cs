namespace Catalog.Contracts.DTOs.Products;

public record UpdateProductVariantRequest(
    long Id,
    string Name,
    string Sku,
    decimal Price,
    int StockQuantity,
    bool IsActive = true,
    int SortOrder = 0
);
