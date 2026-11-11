using Identity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Features.CancelMyOrder;
using Ordering.Application.Features.GetAdminOrderDetail;
using Ordering.Application.Features.GetAdminOrderStatistics;
using Ordering.Application.Features.GetMyOrderDetail;
using Ordering.Application.Features.GetMyOrdersSummary;
using Ordering.Application.Features.GetOrders;
using Ordering.Application.Features.PublicLookupOrder;
using Ordering.Application.Features.UpdateAdminOrderPaymentStatus;
using Ordering.Application.Features.UpdateAdminOrderStatus;
using Ordering.Contracts.DTOs;
using Shared.Web;

namespace Api.Controllers.v1;

[Route("api/v1/orders")]
public class OrderController(ISender sender) : ApiControllerBase
{
    /// <summary>
    /// Lấy danh sách đơn hàng cho cả User và Admin (Tự động nhận diện quyền qua Token / ICurrentUserService)
    /// - Admin: Xem toàn bộ đơn hàng trên hệ thống (hoặc tùy chọn lọc theo userId cụ thể)
    /// - User: Chỉ xem đơn hàng của chính tài khoản mình
    /// Hỗ trợ đầy đủ các tham số lọc tùy chọn: keyword, status, paymentStatus, fulfillmentType, fromDate, toDate, userId, page, pageSize.
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetOrders(
        [FromQuery] string? keyword,
        [FromQuery] string? status,
        [FromQuery] string? paymentStatus,
        [FromQuery] string? fulfillmentType,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] Guid? userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new GetOrdersQuery(
            keyword,
            status,
            paymentStatus,
            fulfillmentType,
            fromDate,
            toDate,
            userId,
            page,
            pageSize);

        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Lấy thống kê số lượng đơn hàng theo từng trạng thái.
    /// Tự động phân quyền thông qua ICurrentUserService: Admin xem toàn hệ thống, User xem đơn của chính mình.
    /// Hỗ trợ param keyword để lọc khi tìm kiếm theo mã đơn hàng.
    /// </summary>
    [HttpGet("summary")]
    [Authorize]
    public async Task<IActionResult> GetOrdersSummary(
        [FromQuery] string? keyword,
        CancellationToken ct = default)
    {
        var query = new GetMyOrdersSummaryQuery(keyword);
        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }

    /// <summary>
    /// [Admin] Thống kê tổng quan đơn hàng và doanh thu hôm nay / trong tháng.
    /// </summary>
    [HttpGet("statistics")]
    [Authorize]
    [HasPermission(PermissionEnum.Order.OrderView)]
    public async Task<IActionResult> GetStatistics(CancellationToken ct = default)
    {
        var query = new GetAdminOrderStatisticsQuery();
        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }

    /// <summary>
    /// [Admin] Lấy chi tiết đơn hàng theo ID (long).
    /// </summary>
    [HttpGet("{id:long}")]
    [Authorize]
    [HasPermission(PermissionEnum.Order.OrderView)]
    public async Task<IActionResult> GetOrderDetailById(
        [FromRoute] long id,
        CancellationToken ct = default)
    {
        var query = new GetAdminOrderDetailQuery(id);
        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Lấy chi tiết đơn hàng theo mã đơn hàng (orderCode).
    /// Tự động phân quyền thông qua ICurrentUserService: Admin xem bất kỳ đơn nào, User chỉ xem đơn của chính mình.
    /// </summary>
    [HttpGet("{orderCode}")]
    [Authorize]
    public async Task<IActionResult> GetOrderDetailByCode(
        [FromRoute] string orderCode,
        CancellationToken ct = default)
    {
        var query = new GetMyOrderDetailQuery(orderCode);
        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }

    /// <summary>
    /// [Admin] Cập nhật trạng thái đơn hàng (Duyệt, làm bánh, giao hàng, hoàn thành, hủy đơn).
    /// </summary>
    [HttpPatch("{id:long}/status")]
    [Authorize]
    [HasPermission(PermissionEnum.Order.OrderUpdate)]
    public async Task<IActionResult> UpdateOrderStatus(
        [FromRoute] long id,
        [FromBody] AdminUpdateOrderStatusRequest request,
        CancellationToken ct = default)
    {
        var command = new UpdateAdminOrderStatusCommand(id, request.Status, request.CancelReason);
        var result = await sender.Send(command, ct);
        return HandleResult(result, "Cập nhật trạng thái đơn hàng thành công.");
    }

    /// <summary>
    /// [Admin] Cập nhật trạng thái thanh toán đơn hàng (Chuyển khoản đã nhận tiền, hoàn tiền...).
    /// </summary>
    [HttpPatch("{id:long}/payment-status")]
    [Authorize]
    [HasPermission(PermissionEnum.Order.OrderUpdate)]
    public async Task<IActionResult> UpdateOrderPaymentStatus(
        [FromRoute] long id,
        [FromBody] AdminUpdateOrderPaymentStatusRequest request,
        CancellationToken ct = default)
    {
        var command = new UpdateAdminOrderPaymentStatusCommand(id, request.PaymentStatus);
        var result = await sender.Send(command, ct);
        return HandleResult(result, "Cập nhật trạng thái thanh toán thành công.");
    }

    /// <summary>
    /// Người dùng hủy đơn hàng của chính mình (Chỉ được hủy khi đơn đang ở trạng thái Pending).
    /// </summary>
    [HttpPatch("{orderCode}/cancel")]
    [Authorize]
    public async Task<IActionResult> CancelOrder(
        [FromRoute] string orderCode,
        [FromBody] CancelOrderRequest request,
        CancellationToken ct = default)
    {
        var command = new CancelMyOrderCommand(orderCode, request.Reason);
        var result = await sender.Send(command, ct);
        return HandleResult(result, "Hủy đơn hàng thành công.");
    }

    /// <summary>
    /// Tra cứu / Theo dõi đơn hàng công khai (Public Tracking) dành cho khách hàng vãng lai (Yêu cầu mã đơn + số điện thoại nhận hàng).
    /// </summary>
    [HttpPost("tracking")]
    [AllowAnonymous]
    public async Task<IActionResult> PublicTracking(
        [FromBody] PublicLookupOrderRequest request,
        CancellationToken ct = default)
    {
        var query = new PublicLookupOrderQuery(request.OrderCode, request.PhoneNumber);
        var result = await sender.Send(query, ct);
        return HandleResult(result);
    }
}
