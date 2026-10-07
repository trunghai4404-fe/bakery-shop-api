using Contents.Domain.Entities;
using Contents.Domain.Enums;
using Contents.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Commons;

namespace Contents.Infrastructure.Persistence.Repositories;

public class BannerRepository(ContentsDbContext context) : IBannerRepository
{
    public async Task<List<Banner>> GetActiveBannersAsync(CancellationToken ct = default)
    {
        var now = DateTimeOffset.UtcNow;

        return await context.Banners
            .AsNoTracking()
            .Where(b => b.IsActive &&
                        (!b.StartDate.HasValue || b.StartDate.Value <= now) &&
                        (!b.EndDate.HasValue || b.EndDate.Value >= now))
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<PagedList<Banner>> GetBannersAsync(
        string? search,
        bool? isActive,
        BannerStatus? status,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = context.Banners
            .AsNoTracking()
            .AsQueryable();

        var now = DateTimeOffset.UtcNow;

        // 1. Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim().ToLower();
            query = query.Where(b => b.Title.ToLower().Contains(searchTerm));
        }

        // 2. IsActive filter
        if (isActive.HasValue)
        {
            query = query.Where(b => b.IsActive == isActive.Value);
        }

        // 3. Status filter using BannerStatus enum
        if (status.HasValue)
        {
            query = status.Value switch
            {
                BannerStatus.Active => query.Where(b => b.IsActive &&
                                                        (!b.StartDate.HasValue || b.StartDate.Value <= now) &&
                                                        (!b.EndDate.HasValue || b.EndDate.Value >= now)),
                BannerStatus.Scheduled => query.Where(b => b.StartDate.HasValue && b.StartDate.Value > now),
                BannerStatus.Expired => query.Where(b => b.EndDate.HasValue && b.EndDate.Value < now),
                BannerStatus.Inactive => query.Where(b => !b.IsActive),
                _ => query
            };
        }

        // 4. Sorting
        query = query.OrderByDescending(b => b.CreatedAt);

        // 5. Total count
        var totalCount = await query.CountAsync(ct);

        // 6. Pagination
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedList<Banner>(items, page, pageSize, totalCount);
    }

    public async Task<Banner?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Banners
            .FirstOrDefaultAsync(b => b.Id == id, ct);
    }

    public async Task<Banner> AddAsync(Banner banner, CancellationToken ct = default)
    {
        await context.Banners.AddAsync(banner, ct);
        return banner;
    }

    public void Update(Banner banner)
    {
        context.Banners.Update(banner);
    }

    public void Delete(Banner banner)
    {
        context.Banners.Remove(banner);
    }
}
