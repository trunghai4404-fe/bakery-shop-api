namespace Catalog.Contracts.DTOs.Products;

public record CreateProductImageRequest(
    string DesktopUrl,
    string MobileUrl,
    int SortOrder = 0,
    bool IsPrimary = false
);
