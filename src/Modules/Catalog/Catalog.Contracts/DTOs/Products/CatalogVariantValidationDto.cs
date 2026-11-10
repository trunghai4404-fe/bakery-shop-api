namespace Catalog.Contracts.DTOs.Products;

public record CatalogVariantValidationDto(
    long VariantId,
    long ProductId,
    string ProductName,
    string VariantName,
    string Sku,
    decimal Price,
    int StockQuantity,
    bool IsActive
);
