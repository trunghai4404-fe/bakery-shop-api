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
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize,
        CancellationToken ct = default
    )
    {
        var query = context.Products
            .AsNoTracking()
            .Include(p => p.ProductImages)
            .Include(p => p.ProductVariants)
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .AsSplitQuery()
            .AsQueryable();

        // 1. Search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim().ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(searchTerm) ||
                p.Sku.ToLower().Contains(searchTerm) ||
                p.Slug.ToLower().Contains(searchTerm));
        }

        // 2. Category filters
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.ProductCategories.Any(c => c.CategoryId == categoryId.Value));
        }

        if (!string.IsNullOrWhiteSpace(categorySlug))
        {
            var slug = categorySlug.Trim().ToLower();
            query = query.Where(p => p.ProductCategories.Any(pc => pc.Category != null && pc.Category.Slug.ToLower() == slug));
        }

        // 3. Status filter
        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        // 4. Sorting
        query = (sortBy?.Trim().ToLower(), isDescending) switch
        {
            ("name", true) => query.OrderByDescending(p => p.Name),
            ("name", false) => query.OrderBy(p => p.Name),
            ("slug", true) => query.OrderByDescending(p => p.Slug),
            ("slug", false) => query.OrderBy(p => p.Slug),
            ("sku", true) => query.OrderByDescending(p => p.Sku),
            ("sku", false) => query.OrderBy(p => p.Sku),
            ("createdat", true) => query.OrderByDescending(p => p.CreatedAt),
            ("createdat", false) => query.OrderBy(p => p.CreatedAt),
            ("id", true) => query.OrderByDescending(p => p.Id),
            ("id", false) => query.OrderBy(p => p.Id),
            _ => isDescending
                ? query.OrderBy(p => p.CreatedAt)
                : query.OrderByDescending(p => p.CreatedAt)
        };

        // 5. Total count
        var totalCount = await query.CountAsync(ct);

        // 6. Pagination
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedList<Product>(items, page, pageSize, totalCount);
    }

    public async Task<Product?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.Products
            .AsNoTracking()
            .Include(p => p.ProductImages)
            .Include(p => p.ProductVariants)
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<Product?> GetByIdForUpdateAsync(long id, CancellationToken ct = default)
    {
        return await context.Products
            .Include(p => p.ProductImages)
            .Include(p => p.ProductVariants)
            .Include(p => p.ProductCategories)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<Product?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var cleanSlug = slug.Trim().ToLower();
        return await context.Products
            .AsNoTracking()
            .Include(p => p.ProductImages)
            .Include(p => p.ProductVariants)
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Slug.ToLower() == cleanSlug, ct);
    }

    public async Task<bool> IsSlugUniqueAsync(string slug, long? excludeId = null, CancellationToken ct = default)
    {
        var cleanSlug = slug.Trim().ToLower();
        return !await context.Products.AnyAsync(
            p => p.Slug.ToLower() == cleanSlug && (!excludeId.HasValue || p.Id != excludeId.Value), ct);
    }

    public async Task<bool> IsSkuUniqueAsync(string sku, long? excludeId = null, CancellationToken ct = default)
    {
        var cleanSku = sku.Trim().ToLower();
        return !await context.Products.AnyAsync(
            p => p.Sku.ToLower() == cleanSku && (!excludeId.HasValue || p.Id != excludeId.Value), ct);
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
