using Basket.Application.Features.CheckoutBasket;
using Basket.Application.Features.ClearBasket;
using Basket.Application.Features.CreateBasket;
using Basket.Application.Features.GetBasketById;
using Basket.Application.Features.UpdateBasket;
using Basket.Contracts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Web;

namespace Api.Controllers.v1;

[Route("api/v1/basket")]
public class BasketController(ISender sender) : ApiControllerBase
{
    /// <summary>
    /// Khởi tạo hoặc lấy giỏ hàng đang active của User / Session
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateBasket([FromBody] CreateBasketRequest request, CancellationToken ct)
    {
        var sessionId = GetSessionId();
        Guid? userId = GetUserId();

        var command = new CreateBasketCommand(userId, sessionId, request);
        var result = await sender.Send(command, ct);
        return HandleResult(result, "Tạo hoặc lấy giỏ hàng thành công.");
    }

    /// <summary>
    /// Lấy chi tiết giỏ hàng theo ID (Tự động Re-validate kiểm tra giá và tồn kho từ Catalog)
    /// </summary>
    [HttpGet("{basketId:guid}")]
    public async Task<IActionResult> GetBasket([FromRoute] Guid basketId, CancellationToken ct)
    {
        var query = new GetBasketByIdQuery(basketId);
        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Cập nhật giỏ hàng: số lượng món, xóa món (số lượng = 0), hình thức nhận hàng (Pickup/Delivery), địa chỉ, thanh toán, vận chuyển, ghi chú
    /// </summary>
    [HttpPut("{basketId:guid}")]
    public async Task<IActionResult> UpdateBasket(
        [FromRoute] Guid basketId,
        [FromBody] UpdateBasketRequest request,
        CancellationToken ct)
    {
        var command = new UpdateBasketCommand(basketId, request);
        var result = await sender.Send(command, ct);
        return HandleResult(result, "Cập nhật giỏ hàng thành công.");
    }

    /// <summary>
    /// Xóa toàn bộ sản phẩm trong giỏ hàng (Clear Basket)
    /// </summary>
    [HttpDelete("{basketId:guid}")]
    public async Task<IActionResult> ClearBasket([FromRoute] Guid basketId, CancellationToken ct)
    {
        var command = new ClearBasketCommand(basketId);
        var result = await sender.Send(command, ct);
        return HandleResult(result, "Xóa sạch giỏ hàng thành công.");
    }

    /// <summary>
    /// Chốt đơn hàng từ giỏ hàng (Tạo Order và Clear giỏ hàng) - Không cần gửi body, lấy toàn bộ thông tin từ Basket
    /// </summary>
    [HttpPost("{basketId:guid}/checkout")]
    public async Task<IActionResult> Checkout([FromRoute] Guid basketId, CancellationToken ct)
    {
        var command = new CheckoutBasketCommand(basketId);
        var result = await sender.Send(command, ct);
        return HandleResult(result, "Đặt hàng thành công.");
    }

    private string? GetSessionId()
    {
        return Request.Headers["X-Session-Id"].FirstOrDefault();
    }
}
