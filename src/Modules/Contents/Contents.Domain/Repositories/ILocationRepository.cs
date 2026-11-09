using Contents.Domain.Entities;

namespace Contents.Domain.Repositories;

public interface ILocationRepository
{
    Task<List<Province>> GetProvincesAsync(CancellationToken ct = default);
    Task<List<District>> GetDistrictsAsync(int? provinceId = null, CancellationToken ct = default);
    Task<List<Ward>> GetWardsAsync(int? districtId = null, int? provinceId = null, CancellationToken ct = default);
    Task<bool> HasLocationsAsync(CancellationToken ct = default);
    Task UpsertLocationsAsync(List<Province> provinces, CancellationToken ct = default);
}
