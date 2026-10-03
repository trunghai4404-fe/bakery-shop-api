using SharedKernel.Commons;
using SharedKernel.Domain;
using SharedKernel.Domain.Entities;
using SharedKernel.Domain.Errors;

namespace Catalog.Domain.Entities;

public class Category : AuditableEntity<long>
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public long? ParentId { get; private set; }
    public string? ImageUrl { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }

    public Category? Parent { get; private set; }
    public ICollection<ProductCategory> ProductCategories { get; private set; } = new List<ProductCategory>();
    public ICollection<Category> Children { get; private set; } = new List<Category>();

    private Category()
    {
    }

    private Category(
        string name,
        string slug,
        string? description,
        long? parentId,
        string? imageUrl,
        bool isActive,
        int sortOrder = 0)
    {
        Name = name;
        Slug = slug;
        Description = description;
        ParentId = parentId;
        ImageUrl = imageUrl;
        IsActive = isActive;
        SortOrder = sortOrder;
    }

    public static Result<Category> Create(
        string name,
        string slug,
        string? description,
        long? parentId,
        string? imageUrl,
        bool isActive,
        int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Category>(Error.Validation(ErrorCode.ValidationError, "Category name is required"));
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            return Result.Failure<Category>(Error.Validation(ErrorCode.ValidationError, "Category slug is required"));
        }

        var category = new Category(name, slug, description, parentId, imageUrl, isActive, sortOrder);
        return Result.Success(category);
    }

    public Result Update(
        string name,
        string slug,
        string? description,
        long? parentId,
        string? imageUrl,
        int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(Error.Validation(ErrorCode.ValidationError, "Category name is required"));
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            return Result.Failure(Error.Validation(ErrorCode.ValidationError, "Category slug is required"));
        }

        Name = name;
        Slug = slug;
        Description = description;
        ParentId = parentId;
        ImageUrl = imageUrl;
        SortOrder = sortOrder;

        return Result.Success();
    }

    public void ToggleStatus()
    {
        IsActive = !IsActive;
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
    }
}
