using Contents.Application.Features.Stores;
using Contents.Contracts.DTOs.Stores;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Web;

namespace Api.Controllers.v1;

[Route("api/v1/stores")]
public class StoreController(ISender sender) : ApiControllerBase
{
    [HttpGet("active")]
    public async Task<IActionResult> GetActive([FromQuery] string? search, [FromQuery] int? provinceId,
        [FromQuery] int? wardId, [FromQuery(Name = "lat")] double? latitude, [FromQuery(Name = "long")] double? longitude,
        [FromQuery] double? radiusKm, [FromQuery] string? sortBy, [FromQuery] bool isDescending = false,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var query = new GetStoresQuery(
            search,
            provinceId,
            wardId,
            true,
            latitude,
            longitude,
            radiusKm,
            sortBy,
            isDescending,
            page,
            pageSize,
            ActiveOnly: true
        );
        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }

    [HttpGet]
    [Authorize]
    [HasPermission(PermissionEnum.Store.StoreView)]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int? provinceId,
        [FromQuery] int? wardId, [FromQuery] bool? isActive, [FromQuery(Name = "lat")] double? latitude,
        [FromQuery(Name = "long")] double? longitude, [FromQuery] double? radiusKm, [FromQuery] string? sortBy,
        [FromQuery] bool isDescending = false, [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var query = new GetStoresQuery(
            search,
            provinceId,
            wardId,
            isActive,
            latitude,
            longitude,
            radiusKm,
            sortBy,
            isDescending,
            page,
            pageSize
        );
        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }

    [HttpGet("{id:long}")]
    [Authorize]
    [HasPermission(PermissionEnum.Store.StoreView)]
    public async Task<IActionResult> GetById(long id, CancellationToken ct) =>
        HandleResult(await sender.Send(new GetStoreByIdQuery(id), ct));

    [HttpPost]
    [Authorize]
    [HasPermission(PermissionEnum.Store.StoreCreate)]
    public async Task<IActionResult> Create([FromBody] StoreRequest request, CancellationToken ct) =>
        HandleResult(await sender.Send(new CreateStoreCommand(request), ct));

    [HttpPut("{id:long}")]
    [Authorize]
    [HasPermission(PermissionEnum.Store.StoreUpdate)]
    public async Task<IActionResult> Update(long id, [FromBody] StoreRequest request, CancellationToken ct) =>
        HandleResult(await sender.Send(new UpdateStoreCommand(id, request), ct));

    [HttpPatch("{id:long}/toggle")]
    [Authorize]
    [HasPermission(PermissionEnum.Store.StoreUpdate)]
    public async Task<IActionResult> Toggle(long id, [FromBody] ToggleStoreRequest? request, CancellationToken ct) =>
        HandleResult(await sender.Send(new ToggleStoreCommand(id, request?.IsActive), ct));

    [HttpDelete("{id:long}")]
    [Authorize]
    [HasPermission(PermissionEnum.Store.StoreDelete)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct) =>
        HandleResult(await sender.Send(new DeleteStoreCommand(id), ct), "Store deleted successfully.");
}
