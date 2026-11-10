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
    /// Khởi tạo hoặc lấy giỏ hàng đang active của Customer / Session
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateBasket([FromBody] CreateBasketRequest request, CancellationToken ct)
    {
        var sessionId = GetSessionId();
        long? customerId = GetCustomerId();

        var command = new CreateBasketCommand(customerId, sessionId, request);
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
    /// Cập nhật giỏ hàng: số lượng món, xóa món (số lượng = 0), hình thức nhận hàng (Pickup/Delivery), địa chỉ, ghi chú
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
    /// Chốt đơn hàng từ giỏ hàng (Tạo Order và Clear giỏ hàng)
    /// </summary>
    [HttpPost("{basketId:guid}/checkout")]
    public async Task<IActionResult> Checkout(
        [FromRoute] Guid basketId,
        [FromBody] CheckoutBasketRequest request,
        CancellationToken ct)
    {
        var command = new CheckoutBasketCommand(basketId, request);
        var result = await sender.Send(command, ct);
        return HandleResult(result, "Đặt hàng thành công.");
    }

    private string? GetSessionId()
    {
        return Request.Headers["X-Session-Id"].FirstOrDefault();
    }

    private long? GetCustomerId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "customerId" || c.Type == "nameid");
        if (claim != null && long.TryParse(claim.Value, out var id))
        {
            return id;
        }
        return null;
    }
}
