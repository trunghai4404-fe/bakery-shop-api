namespace Catalog.Contracts.DTOs.Products;

public class ProductCategoryDto
{
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}
