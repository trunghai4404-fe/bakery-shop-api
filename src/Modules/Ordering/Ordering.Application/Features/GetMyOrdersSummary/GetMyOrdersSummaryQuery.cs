using MediatR;
using Ordering.Contracts.DTOs;
using Ordering.Domain.Enums;
using Ordering.Domain.Repositories;
using SharedKernel.Abstractions;
using SharedKernel.Domain;
using SharedKernel.Domain.Errors;

namespace Ordering.Application.Features.GetMyOrdersSummary;

public record GetMyOrdersSummaryQuery(
    string? Keyword,
    Guid? UserId = null
) : IRequest<Result<OrderSummaryDto>>;

public class GetMyOrdersSummaryQueryHandler(
    IOrderRepository orderRepository,
    ICurrentUserService currentUserService
) : IRequestHandler<GetMyOrdersSummaryQuery, Result<OrderSummaryDto>>
{
    public async Task<Result<OrderSummaryDto>> Handle(GetMyOrdersSummaryQuery request, CancellationToken ct)
    {
        Guid? userId = request.UserId;

        // Tự động phân quyền thông qua ICurrentUserService
        if (!userId.HasValue)
        {
            if (currentUserService.IsAdmin)
            {
                // Admin: Đếm tất cả đơn hàng trên hệ thống
                userId = null;
            }
            else
            {
                // User: Lấy UserId từ Token
                userId = currentUserService.UserId;
                if (!userId.HasValue)
                {
                    return Result.Failure<OrderSummaryDto>(Error.Unauthorized);
                }
            }
        }

        var counts = await orderRepository.GetStatusCountsAsync(userId, request.Keyword, ct);

        int GetCount(OrderStatus status) => counts.TryGetValue(status, out var count) ? count : 0;

        var all = counts.Values.Sum();

        var summary = new OrderSummaryDto(
            all,
            GetCount(OrderStatus.Pending),
            GetCount(OrderStatus.Confirmed),
            GetCount(OrderStatus.Preparing),
            GetCount(OrderStatus.ReadyForPickup),
            GetCount(OrderStatus.Shipping),
            GetCount(OrderStatus.Completed),
            GetCount(OrderStatus.Cancelled)
        );

        return Result.Success(summary);
    }
}
