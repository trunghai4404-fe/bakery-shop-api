using Contents.Domain.Entities;
using Contents.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Commons;

namespace Contents.Infrastructure.Persistence.Repositories;

public class StoreRepository(ContentsDbContext context) : IStoreRepository
{
    public async Task<PagedList<Store>> GetStoresAsync(
        string? search,
        int? provinceId,
        int? wardId,
        bool? isActive,
        double? latitude,
        double? longitude,
        string? sortBy,
        bool isDescending,
        int page,
        int pageSize,
        double? radiusKm = null,
        bool activeOnly = false,
        CancellationToken ct = default
    )
    {
        var query = context.Stores.AsNoTracking().AsQueryable();
        if (activeOnly) query = query.Where(s => s.IsActive);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(s => s.Name.ToLower().Contains(term) || s.Address.ToLower().Contains(term));
        }
        if (provinceId.HasValue) query = query.Where(s => s.ProvinceId == provinceId.Value);
        if (wardId.HasValue) query = query.Where(s => s.WardId == wardId.Value);
        if (isActive.HasValue) query = query.Where(s => s.IsActive == isActive.Value);

        if (latitude.HasValue && longitude.HasValue)
        {
            const double earthRadiusKm = 6371;
            var lat = latitude.Value * Math.PI / 180;
            var lon = longitude.Value * Math.PI / 180;
            var radius = radiusKm ?? double.MaxValue;
            query = query.Where(s => earthRadiusKm * 2 * Math.Asin(Math.Sqrt(
                Math.Pow(Math.Sin((s.Latitude * Math.PI / 180 - lat) / 2), 2) +
                Math.Cos(lat) * Math.Cos(s.Latitude * Math.PI / 180) *
                Math.Pow(Math.Sin((s.Longitude * Math.PI / 180 - lon) / 2), 2))) <= radius);
            query = query.OrderBy(s => earthRadiusKm * 2 * Math.Asin(Math.Sqrt(
                Math.Pow(Math.Sin((s.Latitude * Math.PI / 180 - lat) / 2), 2) +
                Math.Cos(lat) * Math.Cos(s.Latitude * Math.PI / 180) *
                Math.Pow(Math.Sin((s.Longitude * Math.PI / 180 - lon) / 2), 2))));
        }
        else
        {
            query = (sortBy?.ToLower(), isDescending) switch
            {
                ("name", true) => query.OrderByDescending(s => s.Name),
                ("name", false) => query.OrderBy(s => s.Name),
                ("createdat", true) => query.OrderByDescending(s => s.CreatedAt),
                ("createdat", false) => query.OrderBy(s => s.CreatedAt),
                ("sortorder", true) => query.OrderByDescending(s => s.SortOrder).ThenBy(s => s.Name),
                ("sortorder", false) => query.OrderBy(s => s.SortOrder).ThenBy(s => s.Name),
                _ => query.OrderBy(s => s.SortOrder).ThenBy(s => s.Name)
            };
        }

        var totalCount = await query.CountAsync(ct);
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : Math.Min(pageSize, 100);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedList<Store>(items, page, pageSize, totalCount);
    }

    public Task<Store?> GetByIdAsync(long id, CancellationToken ct = default) =>
        context.Stores.FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<bool> ProvinceExistsAsync(int provinceId, CancellationToken ct = default) =>
        context.Provinces.AnyAsync(p => p.Id == provinceId && p.IsActive, ct);

    public Task<bool> WardExistsInProvinceAsync(int wardId, int provinceId, CancellationToken ct = default) =>
        context.Wards.AnyAsync(w => w.Id == wardId && w.IsActive && w.District.ProvinceId == provinceId, ct);

    public async Task AddAsync(Store store, CancellationToken ct = default)
    {
        await context.Stores.AddAsync(store, ct);
    }
    public void Update(Store store) => context.Stores.Update(store);
    public void Delete(Store store) => context.Stores.Remove(store);
}
