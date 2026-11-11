using MediatR;
using Ordering.Contracts.DTOs;
using Ordering.Domain.Enums;
using Ordering.Domain.Repositories;
using SharedKernel.Domain;

namespace Ordering.Application.Features.GetAdminOrderStatistics;

public record GetAdminOrderStatisticsQuery : IRequest<Result<AdminOrderStatisticsDto>>;

public class GetAdminOrderStatisticsQueryHandler(
    IOrderRepository orderRepository
) : IRequestHandler<GetAdminOrderStatisticsQuery, Result<AdminOrderStatisticsDto>>
{
    public async Task<Result<AdminOrderStatisticsDto>> Handle(GetAdminOrderStatisticsQuery request, CancellationToken ct)
    {
        var (statusCounts, todayRevenue, monthRevenue, totalOrders) = await orderRepository.GetAdminStatisticsAsync(ct);

        int GetCount(OrderStatus status) => statusCounts.TryGetValue(status, out var count) ? count : 0;

        var dto = new AdminOrderStatisticsDto(
            totalOrders,
            GetCount(OrderStatus.Pending),
            GetCount(OrderStatus.Confirmed),
            GetCount(OrderStatus.Preparing),
            GetCount(OrderStatus.ReadyForPickup),
            GetCount(OrderStatus.Shipping),
            GetCount(OrderStatus.Completed),
            GetCount(OrderStatus.Cancelled),
            todayRevenue,
            monthRevenue
        );

        return Result.Success(dto);
    }
}
