using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Features.GetPaymentMethods;
using Ordering.Application.Features.TogglePaymentMethod;
using Ordering.Contracts.DTOs;
using Shared.Web;

namespace Api.Controllers.v1;

[Route("api/v1/payment-methods")]
public class PaymentMethodController(ISender sender) : ApiControllerBase
{
    /// <summary>
    /// Lấy danh sách các phương thức thanh toán (mặc định chỉ lấy active)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPaymentMethods([FromQuery] bool? onlyActive = true, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetPaymentMethodsQuery(onlyActive), ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Bật / Tắt (Toggle) trạng thái hoạt động của phương thức thanh toán
    /// </summary>
    [HttpPatch("{code}/toggle")]
    public async Task<IActionResult> Toggle(
        [FromRoute] string code,
        [FromBody] TogglePaymentMethodRequest? request,
        CancellationToken ct)
    {
        var command = new TogglePaymentMethodCommand(code, request?.IsEnabled);
        var result = await sender.Send(command, ct);
        return HandleResult(result, "Cập nhật trạng thái phương thức thanh toán thành công.");
    }
}
