using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Commons;

namespace Catalog.Infrastructure.Persistence.Repositories;

public class ProductRepository(CatalogDbContext context) : IProductRepository
{
    public async Task<PagedList<Product>> GetProductsAsync(
        string? search,
        long? categoryId,
        string? categorySlug,
        bool? isActive,
        int page, 
        int pageSize,
        CancellationToken ct = default
    )
    {
        var query = context.Products
            .AsQueryable()
            .AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(searchTerm) 
                                     || p.Sku.ToLower().Contains(searchTerm));
            
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.ProductCategories.Any(c => c.CategoryId == categoryId.Value));
        }

        if (!string.IsNullOrWhiteSpace(categorySlug))
        {
            query = query.Where(p => p.ProductCategories.Any(pc => pc.Category.Slug == categorySlug));
        }

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }
        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        
        return new PagedList<Product>(items, page, pageSize, totalCount);
    }

    public async Task<Product?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<bool> IsSlugUniqueAsync(string slug, CancellationToken ct = default)
    {
        return !await context.Products.AnyAsync(p => p.Slug == slug, ct);
    }

    public async Task<bool> IsSkuUniqueAsync(string sku, CancellationToken ct = default)
    {
        return !await context.Products.AnyAsync(p => p.Sku == sku, ct);
    }

    public async Task<Product> AddAsync(Product product, CancellationToken ct = default)
    {
        await context.Products.AddAsync(product, ct);
        return product;
    }

    public void Update(Product product)
    {
        context.Products.Update(product);
    }

    public void Delete(Product product)
    {
        context.Products.Remove(product);
    }
}
