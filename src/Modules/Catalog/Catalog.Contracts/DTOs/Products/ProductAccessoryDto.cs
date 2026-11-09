namespace Catalog.Contracts.DTOs.Products;

public class ProductAccessoryDto
{
    public long AccessoryProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public int DefaultQuantity { get; set; }
    public bool IsIncluded { get; set; }
    public int SortOrder { get; set; }
    public bool IsMultiSelect { get; set; }
    public int? MaxSelectCount { get; set; }
    public bool IsRequired { get; set; }
    public List<ProductVariantDto> Variants { get; set; } = new();
}
