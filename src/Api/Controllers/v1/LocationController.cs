using Contents.Application.Features.Locations.Commands.SyncLocationsCommand;
using Contents.Application.Features.Locations.Queries.GetDistrictsByProvinceQuery;
using Contents.Application.Features.Locations.Queries.GetProvincesQuery;
using Contents.Application.Features.Locations.Queries.GetWardsByDistrictQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Web;

namespace Api.Controllers.v1;

[Route("api/v1/locations")]
public class LocationController(ISender sender) : ApiControllerBase
{
    /// <summary>
    /// Lấy danh sách 63 Tỉnh/Thành phố (Được Cache tại Redis)
    /// </summary>
    [HttpGet("provinces")]
    public async Task<IActionResult> GetProvinces(CancellationToken ct)
    {
        var result = await sender.Send(new GetProvincesQuery(), ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Lấy danh sách Quận/Huyện (Hỗ trợ lọc theo provinceId, nếu không truyền sẽ lấy toàn bộ)
    /// </summary>
    [HttpGet("districts")]
    public async Task<IActionResult> GetDistricts([FromQuery] int? provinceId, CancellationToken ct)
    {
        var result = await sender.Send(new GetDistrictsByProvinceQuery(provinceId), ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Giữ tương thích ngược: Lấy danh sách Quận/Huyện theo path provinceId
    /// </summary>
    [HttpGet("provinces/{provinceId:int}/districts")]
    public async Task<IActionResult> GetDistrictsByProvince([FromRoute] int provinceId, CancellationToken ct)
    {
        var result = await sender.Send(new GetDistrictsByProvinceQuery(provinceId), ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Lấy danh sách Phường/Xã (Hỗ trợ lọc theo districtId hoặc provinceId, nếu không truyền sẽ lấy toàn bộ)
    /// </summary>
    [HttpGet("wards")]
    public async Task<IActionResult> GetWards(
        [FromQuery] int? districtId,
        [FromQuery] int? provinceId,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetWardsByDistrictQuery(districtId, provinceId), ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Giữ tương thích ngược: Lấy danh sách Phường/Xã theo path districtId
    /// </summary>
    [HttpGet("districts/{districtId:int}/wards")]
    public async Task<IActionResult> GetWardsByDistrict([FromRoute] int districtId, CancellationToken ct)
    {
        var result = await sender.Send(new GetWardsByDistrictQuery(districtId), ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Đồng bộ danh mục hành chính từ provinces.open-api.vn vào Database và làm ấm Cache Redis
    /// </summary>
    [HttpPost("sync")]
    public async Task<IActionResult> SyncLocations(CancellationToken ct)
    {
        var result = await sender.Send(new SyncLocationsCommand(), ct);
        return HandleResult(result, "Đồng bộ danh mục địa giới hành chính thành công.");
    }
}
