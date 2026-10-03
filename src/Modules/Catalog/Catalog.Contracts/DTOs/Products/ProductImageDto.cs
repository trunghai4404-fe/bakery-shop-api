namespace Catalog.Contracts.DTOs.Products;

public class ProductImageDto
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public string DesktopUrl { get; set; } = string.Empty;
    public string MobileUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
}
