using Contents.Contracts.DTOs.Locations;

namespace Contents.Contracts.Interfaces;

public interface ILocationService
{
    Task<List<ProvinceDto>> GetProvincesAsync(CancellationToken ct = default);
    Task<List<DistrictDto>> GetDistrictsAsync(int? provinceId = null, CancellationToken ct = default);
    Task<List<WardDto>> GetWardsAsync(int? districtId = null, int? provinceId = null, CancellationToken ct = default);
    Task<int> SyncLocationsFromOpenApiAsync(CancellationToken ct = default);
}
