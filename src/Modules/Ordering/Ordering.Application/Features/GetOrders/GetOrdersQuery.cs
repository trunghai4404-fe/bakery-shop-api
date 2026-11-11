using MediatR;
using Ordering.Application.Common.Mappings;
using Ordering.Contracts.DTOs;
using Ordering.Domain.Enums;
using Ordering.Domain.Repositories;
using SharedKernel.Abstractions;
using SharedKernel.Commons;
using SharedKernel.Domain;
using SharedKernel.Domain.Errors;

namespace Ordering.Application.Features.GetOrders;

public record GetOrdersQuery(
    string? Keyword = null,
    string? Status = null,
    string? PaymentStatus = null,
    string? FulfillmentType = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    Guid? UserId = null,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PagedList<OrderDto>>>;

public class GetOrdersQueryHandler(
    IOrderRepository orderRepository,
    ICurrentUserService currentUserService
) : IRequestHandler<GetOrdersQuery, Result<PagedList<OrderDto>>>
{
    public async Task<Result<PagedList<OrderDto>>> Handle(GetOrdersQuery request, CancellationToken ct)
    {
        Guid? targetUserId = request.UserId;

        // Tự động phân quyền dựa vào ICurrentUserService:
        // - Admin: Được phép xem tất cả đơn hàng (targetUserId = null) hoặc lọc theo UserId của khách hàng cụ thể (nếu request.UserId có giá trị).
        // - User thông thường: Bắt buộc chỉ được xem đơn hàng của chính tài khoản mình (UserId lấy từ Token).
        if (currentUserService.IsAdmin)
        {
            // Admin có thể truyền UserId hoặc để null để xem toàn bộ
            targetUserId = request.UserId;
        }
        else
        {
            var currentUserId = currentUserService.UserId;
            if (!currentUserId.HasValue)
            {
                return Result.Failure<PagedList<OrderDto>>(Error.Unauthorized);
            }

            // Ghi đè bắt buộc UserId của chính User hiện tại
            targetUserId = currentUserId.Value;
        }

        OrderStatus? orderStatus = null;
        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<OrderStatus>(request.Status, true, out var parsedStatus))
        {
            orderStatus = parsedStatus;
        }

        PaymentStatus? paymentStatus = null;
        if (!string.IsNullOrWhiteSpace(request.PaymentStatus) && Enum.TryParse<PaymentStatus>(request.PaymentStatus, true, out var parsedPayment))
        {
            paymentStatus = parsedPayment;
        }

        var (items, totalCount) = await orderRepository.GetOrdersPagedAsync(
            targetUserId,
            request.Keyword,
            orderStatus,
            paymentStatus,
            request.FulfillmentType,
            request.FromDate,
            request.ToDate,
            request.Page,
            request.PageSize,
            ct);

        var dtos = items.Select(o => o.ToOrderDto()).ToList();
        var paged = new PagedList<OrderDto>(dtos, request.Page, request.PageSize, totalCount);

        return Result.Success(paged);
    }
}
