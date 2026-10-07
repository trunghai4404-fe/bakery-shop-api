using Contents.Application.Features.Banners.Commands.CreateBannerCommand;
using Contents.Application.Features.Banners.Commands.DeleteBannerCommand;
using Contents.Application.Features.Banners.Commands.ToggleBannerCommand;
using Contents.Application.Features.Banners.Commands.UpdateBannerCommand;
using Contents.Application.Features.Banners.Queries.GetActiveBannersQuery;
using Contents.Application.Features.Banners.Queries.GetBannerByIdQuery;
using Contents.Application.Features.Banners.Queries.GetBannersQuery;
using Contents.Contracts.DTOs.Banners;
using Contents.Domain.Enums;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Web;

namespace Api.Controllers.v1;

[Route("api/v1/banners")]
public class BannerController(ISender sender) : ApiControllerBase
{
    /// <summary>
    /// Lấy danh sách banner đang active dành cho Web Storefront
    /// </summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveBanners(CancellationToken ct = default)
    {
        var query = new GetActiveBannersQuery();
        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Lấy danh sách banner có phân trang, tìm kiếm và lọc trạng thái dành cho CMS
    /// </summary>
    [HttpGet]
    [Authorize]
    [HasPermission(PermissionEnum.Banner.BannerView)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] bool? isActive,
        [FromQuery] BannerStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new GetBannersQuery(search, isActive, status, page, pageSize);
        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Lấy chi tiết banner theo Id
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct = default)
    {
        var query = new GetBannerByIdQuery(id);
        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Tạo mới banner (CMS)
    /// </summary>
    [HttpPost]
    [Authorize]
    [HasPermission(PermissionEnum.Banner.BannerCreate)]
    public async Task<IActionResult> Create([FromBody] CreateBannerRequest request, CancellationToken ct)
    {
        var command = new CreateBannerCommand(
            request.Title,
            request.ImageUrl,
            request.MobileImageUrl,
            (BannerActionType)request.ActionType,
            request.TargetValue,
            request.OpenInNewTab,
            request.IsActive,
            request.StartDate,
            request.EndDate,
            request.Metadata
        );

        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Cập nhật banner (CMS)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize]
    [HasPermission(PermissionEnum.Banner.BannerUpdate)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateBannerRequest request, CancellationToken ct)
    {
        var command = new UpdateBannerCommand(
            id,
            request.Title,
            request.ImageUrl,
            request.MobileImageUrl,
            (BannerActionType)request.ActionType,
            request.TargetValue,
            request.OpenInNewTab,
            request.IsActive,
            request.StartDate,
            request.EndDate,
            request.Metadata
        );

        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Bật/Tắt trạng thái hoạt động của banner (CMS)
    /// </summary>
    [HttpPatch("{id:guid}/toggle")]
    [Authorize]
    [HasPermission(PermissionEnum.Banner.BannerUpdate)]
    public async Task<IActionResult> Toggle([FromRoute] Guid id, [FromBody] ToggleBannerRequest? request, CancellationToken ct)
    {
        var command = new ToggleBannerCommand(id, request?.IsActive);
        var result = await sender.Send(command, ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Xóa banner (CMS - Soft Delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [HasPermission(PermissionEnum.Banner.BannerDelete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var command = new DeleteBannerCommand(id);
        var result = await sender.Send(command, ct);
        return HandleResult(result, "Banner deleted successfully.");
    }
}
