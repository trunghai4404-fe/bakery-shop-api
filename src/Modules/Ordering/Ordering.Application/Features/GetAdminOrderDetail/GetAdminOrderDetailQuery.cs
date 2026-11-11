using MediatR;
using Ordering.Application.Common.Mappings;
using Ordering.Contracts.DTOs;
using Ordering.Domain.Errors;
using Ordering.Domain.Repositories;
using SharedKernel.Domain;

namespace Ordering.Application.Features.GetAdminOrderDetail;

public record GetAdminOrderDetailQuery(long Id) : IRequest<Result<OrderDto>>;

public class GetAdminOrderDetailQueryHandler(
    IOrderRepository orderRepository
) : IRequestHandler<GetAdminOrderDetailQuery, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(GetAdminOrderDetailQuery request, CancellationToken ct)
    {
        var order = await orderRepository.GetByIdAsync(request.Id, ct);
        if (order == null)
        {
            return Result.Failure<OrderDto>(OrderingErrors.NotFound(request.Id));
        }

        return Result.Success(order.ToOrderDto());
    }
}
