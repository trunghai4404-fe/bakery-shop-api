using Contents.Contracts.DTOs.Locations;

namespace Contents.Contracts.Interfaces;

public interface ILocationService
{
    Task<List<ProvinceDto>> GetProvincesAsync(string? keyword = null, CancellationToken ct = default);
    Task<List<DistrictDto>> GetDistrictsAsync(int? provinceId = null, string? keyword = null, CancellationToken ct = default);
    Task<List<WardDto>> GetWardsAsync(int? districtId = null, int? provinceId = null, string? keyword = null, CancellationToken ct = default);
    Task<int> SyncLocationsFromOpenApiAsync(CancellationToken ct = default);
}
