namespace Catalog.Contracts.DTOs.Products;

public class ProductVariantDto
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}
