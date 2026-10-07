using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Commons;

namespace Catalog.Infrastructure.Persistence.Repositories;

public class CategoryRepository(CatalogDbContext context) : ICategoryRepository
{
    public async Task<PagedList<Category>> GetCategoriesAsync(
        string? search,
        bool? isActive,
        long? parentId,
        bool? isRoot,
        bool? isFeatured,
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = context.Categories
            .AsNoTracking()
            .Include(c => c.Parent)
            .Include(c => c.Children)
            .AsQueryable();

        // 1. Search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim().ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(searchTerm) ||
                c.Slug.ToLower().Contains(searchTerm) ||
                (c.Description != null && c.Description.ToLower().Contains(searchTerm)));
        }

        // 2. Status filter
        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }

        // 3. Featured filter
        if (isFeatured.HasValue)
        {
            query = query.Where(c => c.IsFeatured == isFeatured.Value);
        }

        // 4. Parent / Hierarchy filter
        if (parentId.HasValue)
        {
            query = query.Where(c => c.ParentId == parentId.Value);
        }
        else if (isRoot.HasValue)
        {
            query = isRoot.Value
                ? query.Where(c => c.ParentId == null)
                : query.Where(c => c.ParentId != null);
        }

        // 5. Sorting
        query = (sortBy?.ToLower(), isDescending) switch
        {
            ("name", true) => query.OrderByDescending(c => c.Name),
            ("name", false) => query.OrderBy(c => c.Name),
            ("slug", true) => query.OrderByDescending(c => c.Slug),
            ("slug", false) => query.OrderBy(c => c.Slug),
            ("createdat", true) => query.OrderByDescending(c => c.CreatedAt),
            ("createdat", false) => query.OrderBy(c => c.CreatedAt),
            ("id", true) => query.OrderByDescending(c => c.Id),
            ("id", false) => query.OrderBy(c => c.Id),
            ("sortorder", true) => query.OrderByDescending(c => c.SortOrder),
            ("sortorder", false) => query.OrderBy(c => c.SortOrder),
            _ => isDescending
                ? query.OrderByDescending(c => c.SortOrder).ThenByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.SortOrder).ThenByDescending(c => c.CreatedAt)
        };

        // 6. Total count
        var totalCount = await query.CountAsync(ct);

        // 7. Pagination
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedList<Category>(items, page, pageSize, totalCount);
    }

    public async Task<List<Category>> GetListAsync(bool? isActive = null, bool? isFeatured = null, CancellationToken ct = default)
    {
        var query = context.Categories
            .AsNoTracking()
            .AsQueryable();

        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }

        if (isFeatured.HasValue)
        {
            query = query.Where(c => c.IsFeatured == isFeatured.Value);
        }

        return await query
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<List<Category>> GetRootCategoriesAsync(CancellationToken ct = default)
    {
        return await context.Categories
            .AsNoTracking()
            .Where(c => c.ParentId == null)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(ct);
    }

    public async Task<List<Category>> GetAllCategoriesAsync(CancellationToken ct = default)
    {
        var allCategories = await context.Categories
            .AsNoTracking()
            .OrderBy(c => c.SortOrder)
            .ToListAsync(ct);
        
        var lookup = allCategories.ToLookup(c => c.ParentId);
        foreach (var category in allCategories)
        {
            var children = lookup[category.Id];
            foreach (var child in children)
            {
                category.Children.Add(child);
            }
        }

        return allCategories
            .Where(c => c.ParentId == null)
            .ToList();
    }

    public async Task<Category?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await context.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Category?> GetByIdWithDetailsAsync(long id, CancellationToken ct = default)
    {
        return await context.Categories
            .AsNoTracking()
            .Include(c => c.Parent)
            .Include(c => c.Children)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<bool> HasProductsAsync(long categoryId, CancellationToken ct = default)
    {
        return await context.ProductCategories
            .AnyAsync(pc => pc.CategoryId == categoryId, ct);
    }

    public async Task<bool> HasChildrenAsync(long categoryId, CancellationToken ct = default)
    {
        return await context.Categories
            .AnyAsync(c => c.ParentId == categoryId, ct);
    }

    public async Task<bool> IsSlugUniqueAsync(string slug, long? excludeId = null, CancellationToken ct = default)
    {
        return !await context.Categories
            .AnyAsync(c => c.Slug == slug && (!excludeId.HasValue || c.Id != excludeId.Value), ct);
    }

    public async Task<Category> AddAsync(Category category, CancellationToken ct = default)
    {
        await context.Categories.AddAsync(category, ct);
        return category;
    }

    public void Update(Category category)
    {
        context.Categories.Update(category);
    }

    public void Delete(Category category)
    {
        context.Categories.Remove(category);
    }
}
