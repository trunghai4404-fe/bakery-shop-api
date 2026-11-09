namespace Catalog.Contracts.DTOs.Categories;

public record UpdateCategoryRequest(
    string Name,
    string? Slug = null,
    string? Description = null,
    long? ParentId = null,
    string? ImageUrl = null,
    bool IsFeatured = true,
    int SortOrder = 0
);
