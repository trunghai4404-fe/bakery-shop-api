namespace Catalog.Contracts.DTOs.Categories;

public class CreateCategoryResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long? ParentId { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}
