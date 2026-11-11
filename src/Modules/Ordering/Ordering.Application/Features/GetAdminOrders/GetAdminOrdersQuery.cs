using MediatR;
using Ordering.Application.Common.Mappings;
using Ordering.Contracts.DTOs;
using Ordering.Domain.Enums;
using Ordering.Domain.Repositories;
using SharedKernel.Commons;
using SharedKernel.Domain;

namespace Ordering.Application.Features.GetAdminOrders;

public record GetAdminOrdersQuery(
    string? Keyword,
    string? Status,
    string? PaymentStatus,
    string? FulfillmentType,
    DateTime? FromDate,
    DateTime? ToDate,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PagedList<OrderDto>>>;

public class GetAdminOrdersQueryHandler(
    IOrderRepository orderRepository
) : IRequestHandler<GetAdminOrdersQuery, Result<PagedList<OrderDto>>>
{
    public async Task<Result<PagedList<OrderDto>>> Handle(GetAdminOrdersQuery request, CancellationToken ct)
    {
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

        var (items, totalCount) = await orderRepository.GetAdminPagedAsync(
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
