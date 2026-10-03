namespace Catalog.Contracts.DTOs.Products;

public class UpdateProductResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
