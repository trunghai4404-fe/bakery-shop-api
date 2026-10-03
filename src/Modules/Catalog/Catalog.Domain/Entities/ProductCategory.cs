namespace Catalog.Domain.Entities;

public class ProductCategory
{
    public long ProductId { get; private set; }
    public Product? Product { get; private set; }
    public long CategoryId { get; private set; }
    public Category? Category { get; private set; }
    
    public int SortOrder { get; private set; }
    
    private ProductCategory() { }

    public ProductCategory(long productId, long categoryId)
    {
        ProductId = productId;
        CategoryId = categoryId;
    }
}
