using SharedKernel.Domain;
using SharedKernel.Domain.Entities;
using SharedKernel.Domain.Interfaces;

namespace Catalog.Domain.Entities;

public class ProductVariant : AuditableEntity<long>, IAuditableEntity
{
    public long ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public string Sku { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }
    public int SortOrder { get; private set; }
    
    private  ProductVariant()
    {
    }

    internal ProductVariant(long productId, string name, string sku, decimal price, int stockQuantity)
    {
           ProductId = productId;
           Name = name;
           Sku = sku;
           Price = price;
           StockQuantity = stockQuantity;
           IsActive = true;
    }

    internal void Update(string name, string sku, decimal price, int stockQuantity, bool isActive)
    {
        Name = name;
        Sku = sku;
        Price = price;
        StockQuantity = stockQuantity;
        IsActive = isActive;
    }

}