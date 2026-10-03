using Catalog.Domain.Entities;
using SharedKernel.Commons;

namespace Catalog.Domain.Repositories;

public interface IProductRepository
{
    Task<PagedList<Product>> GetProductsAsync(
        string? search,
        long? categoryId,
        string? categorySlug,
        bool? isActive,
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize,
        CancellationToken ct = default
    );
    Task<Product?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Product?> GetByIdForUpdateAsync(long id, CancellationToken ct = default);
    Task<Product?> GetBySlugAsync(string slug, CancellationToken ct = default);
    
    Task<bool> IsSlugUniqueAsync(string slug, long? excludeId = null, CancellationToken ct = default);
    Task<bool> IsSkuUniqueAsync(string sku, long? excludeId = null, CancellationToken ct = default);
    
    Task<Product> AddAsync(Product product, CancellationToken ct = default);
    void Update(Product product);
    void Delete(Product product);
}
