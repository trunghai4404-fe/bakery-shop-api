using MediatR;
using Ordering.Application.Common.Mappings;
using Ordering.Contracts.DTOs;
using Ordering.Domain.Enums;
using Ordering.Domain.Repositories;
using SharedKernel.Commons;
using SharedKernel.Domain;

namespace Ordering.Application.Features.GetMyOrders;

public record GetMyOrdersQuery(
    Guid UserId,
    string? Keyword,
    string? Status,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PagedList<UserOrderSummaryDto>>>;

public class GetMyOrdersQueryHandler(
    IOrderRepository orderRepository
) : IRequestHandler<GetMyOrdersQuery, Result<PagedList<UserOrderSummaryDto>>>
{
    public async Task<Result<PagedList<UserOrderSummaryDto>>> Handle(GetMyOrdersQuery request, CancellationToken ct)
    {
        OrderStatus? orderStatus = null;
        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<OrderStatus>(request.Status, true, out var parsed))
        {
            orderStatus = parsed;
        }

        var (items, totalCount) = await orderRepository.GetByUserIdPagedAsync(
            request.UserId,
            request.Keyword,
            orderStatus,
            request.Page,
            request.PageSize,
            ct);

        var dtos = items.Select(o => o.ToUserSummaryDto()).ToList();
        var paged = new PagedList<UserOrderSummaryDto>(dtos, request.Page, request.PageSize, totalCount);

        return Result.Success(paged);
    }
}
