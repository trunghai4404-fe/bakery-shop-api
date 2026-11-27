using Contents.Domain.Entities;
using SharedKernel.Commons;

namespace Contents.Domain.Repositories;

public interface IStoreRepository
{
    Task<PagedList<Store>> GetStoresAsync(
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
    );
    Task<Store?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<bool> ProvinceExistsAsync(int provinceId, CancellationToken ct = default);
    Task<bool> WardExistsInProvinceAsync(int wardId, int provinceId, CancellationToken ct = default);
    Task AddAsync(Store store, CancellationToken ct = default);
    void Update(Store store);
    void Delete(Store store);
}
