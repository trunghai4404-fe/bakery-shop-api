namespace Catalog.Contracts.DTOs.Products;

public record ProductAccessoryRequest(
    long AccessoryProductId,
    int DefaultQuantity = 1,
    bool IsIncluded = true,
    int SortOrder = 0,
    bool IsMultiSelect = false,
    int? MaxSelectCount = null,
    bool IsRequired = false
);
