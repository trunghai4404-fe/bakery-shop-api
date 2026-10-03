using Catalog.Domain.Entities;
using SharedKernel.Commons;

namespace Catalog.Domain.Repositories;

public interface ICategoryRepository
{
    Task<PagedList<Category>> GetCategoriesAsync(
        string? search,
        bool? isActive,
        long? parentId,
        bool? isRoot,
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize,
        CancellationToken ct = default
    );
    Task<List<Category>> GetListAsync(bool? isActive = null, CancellationToken ct = default);
    Task<List<Category>> GetAllCategoriesAsync(CancellationToken ct = default);
    Task<List<Category>> GetRootCategoriesAsync(CancellationToken ct = default);
    Task<Category?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Category?> GetByIdWithDetailsAsync(long id, CancellationToken ct = default);
    
    Task<bool> HasProductsAsync(long categoryId, CancellationToken ct = default);
    Task<bool> HasChildrenAsync(long categoryId, CancellationToken ct = default);
    Task<bool> IsSlugUniqueAsync(string slug, long? excludeId = null, CancellationToken ct = default);
    
    Task<Category> AddAsync(Category category, CancellationToken ct = default);
    void Update(Category category);
    void Delete(Category category);
}
