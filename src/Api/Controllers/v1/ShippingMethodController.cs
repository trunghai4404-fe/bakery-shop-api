using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Features.GetShippingMethods;
using Ordering.Application.Features.ToggleShippingMethod;
using Ordering.Contracts.DTOs;
using Shared.Web;

namespace Api.Controllers.v1;

[Route("api/v1/shipping-methods")]
public class ShippingMethodController(ISender sender) : ApiControllerBase
{
    /// <summary>
    /// Lấy danh sách các phương thức vận chuyển (mặc định chỉ lấy active)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetShippingMethods([FromQuery] bool? onlyActive = true, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetShippingMethodsQuery(onlyActive), ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Bật / Tắt (Toggle) trạng thái hoạt động của phương thức vận chuyển
    /// </summary>
    [HttpPatch("{code}/toggle")]
    public async Task<IActionResult> Toggle(
        [FromRoute] string code,
        [FromBody] ToggleShippingMethodRequest? request,
        CancellationToken ct)
    {
        var command = new ToggleShippingMethodCommand(code, request?.IsEnabled);
        var result = await sender.Send(command, ct);
        return HandleResult(result, "Cập nhật trạng thái phương thức vận chuyển thành công.");
    }
}
