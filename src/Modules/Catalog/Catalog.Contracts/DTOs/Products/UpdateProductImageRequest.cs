namespace Catalog.Contracts.DTOs.Products;

public record UpdateProductImageRequest(
    long Id,
    string DesktopUrl,
    string MobileUrl,
    int SortOrder = 0,
    bool IsPrimary = false
);
