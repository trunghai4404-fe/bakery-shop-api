using SharedKernel.Commons;
using SharedKernel.Domain;
using SharedKernel.Domain.Entities;
using SharedKernel.Domain.Errors;
using SharedKernel.Domain.Interfaces;

namespace Catalog.Domain.Entities;

public class Product : AuditableEntity<long>, IAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Sku { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    // public long PromotionId {get; private set;}
    
    public ICollection<ProductCategory>  ProductCategories { get; private set; } = new List<ProductCategory>();
    public ICollection<ProductVariant>   ProductVariants { get; private set; } = new List<ProductVariant>();
    public ICollection<ProductImage> ProductImages { get; private set; } = new List<ProductImage>();
    
    public record ImageData(long Id, string DesktopUrl, string MobileUrl, int SortOrder, bool IsPrimary);

    private Product()
    { }

    private Product(string name, string slug, string sku, string? description, bool isActive)
    {
        Name = name;
        Slug = slug;
        Sku = sku;
        Description = description;
        IsActive = isActive;
    }

    public static Result<Product> Create(
        string name,
        string slug,
        string sku,
        string? description,
        bool isActive
    )
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Product>(Error.Validation(ErrorCode.ValidationError, "Product name is required"));
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            return Result.Failure<Product>(Error.Validation(ErrorCode.ValidationError, "Product slug is required"));
        }
        if (string.IsNullOrWhiteSpace(sku))
        {
            return Result.Failure<Product>(Error.Validation(ErrorCode.ValidationError, "Product sku is required"));
        }
        
        var product = new Product(name, slug, sku,description, isActive);
        return Result.Success(product);
    }

    public void Update(string? name, string? slug, string? sku, string? description, bool? isActive)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }

        if (!string.IsNullOrWhiteSpace(slug))
        {
            Slug = slug;
        }

        if (!string.IsNullOrWhiteSpace(sku))
        {
            Sku = sku;
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
            Description = description;
        }
        if (isActive.HasValue)
        {
            IsActive = isActive.Value;
        }
    }
    
    public void AddVariant(string name, string sku, decimal price, int stockQuantity)
    {
        var variant = new ProductVariant(Id, name, sku, price, stockQuantity);
        ProductVariants.Add(variant);
    }

    public Result UpdateVariant(long variantId, string name, string sku, decimal price, int stockQuantity, bool isActive)
    {
        var variant = ProductVariants.FirstOrDefault(v => v.Id == variantId);
        if (variant == null)
        {
            return Result.Failure(Error.NotFound(ErrorCode.VariantNotFound, $"Variant with id {variantId} not found"));
        }
        variant.Update(name, sku, price, stockQuantity, isActive);
        return Result.Success();
    }

    public void AddImage(string desktopUrl, string mobileUrl, int sortOrder,  bool isPrimary)
    {
        var image = new ProductImage(Id, desktopUrl, mobileUrl, sortOrder, isPrimary);
        ProductImages.Add(image);
    }
    
    public void SyncImages(IReadOnlyCollection<ImageData> newImageDatas)
    {
        var incomingIds = newImageDatas.Where(x => x.Id > 0).Select(x => x.Id).ToList();

        var imagesToRemove = ProductImages.Where(img => !incomingIds.Contains(img.Id)).ToList();
        foreach (var img in imagesToRemove)
        {
            ProductImages.Remove(img); 
        }

        foreach (var data in newImageDatas)
        {
            if (data.Id == 0)
            {
                var newImg = new ProductImage(Id, data.DesktopUrl, data.MobileUrl, data.SortOrder, data.IsPrimary);
                ProductImages.Add(newImg);
            }
            else
            {
                var existingImg = ProductImages.FirstOrDefault(x => x.Id == data.Id);
                if (existingImg != null)
                {
                    existingImg.UpdateImage(data.DesktopUrl, data.MobileUrl, data.SortOrder, data.IsPrimary);
                }
            }
        }
    }
    
    
    public void AddCategory(long categoryId)
    {
        if (ProductCategories.All(c => c.CategoryId != categoryId))
        {
            ProductCategories.Add(new ProductCategory(Id, categoryId));
        }
    }
}