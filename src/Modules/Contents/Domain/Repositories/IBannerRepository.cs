using Contents.Domain.Entities;
using Contents.Domain.Enums;
using SharedKernel.Commons;

namespace Contents.Domain.Repositories;

public interface IBannerRepository
{
    Task<List<Banner>> GetActiveBannersAsync(CancellationToken ct = default);
    Task<PagedList<Banner>> GetBannersAsync(
        string? search,
        bool? isActive,
        BannerStatus? status,
        int page,
        int pageSize,
        CancellationToken ct = default);
    Task<Banner?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Banner> AddAsync(Banner banner, CancellationToken ct = default);
    void Update(Banner banner);
    void Delete(Banner banner);
}
