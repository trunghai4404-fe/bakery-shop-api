using MediatR;
using Ordering.Application.Common.Mappings;
using Ordering.Contracts.DTOs;
using Ordering.Domain.Errors;
using Ordering.Domain.Repositories;
using SharedKernel.Domain;

namespace Ordering.Application.Features.GetAdminOrderDetail;

public record GetAdminOrderDetailQuery(long Id) : IRequest<Result<OrderDetailDto>>;

public class GetAdminOrderDetailQueryHandler(
    IOrderRepository orderRepository
) : IRequestHandler<GetAdminOrderDetailQuery, Result<OrderDetailDto>>
{
    public async Task<Result<OrderDetailDto>> Handle(GetAdminOrderDetailQuery request, CancellationToken ct)
    {
        var order = await orderRepository.GetDetailByIdAsync(request.Id, ct);
        if (order == null)
        {
            return Result.Failure<OrderDetailDto>(OrderingErrors.NotFound(request.Id));
        }

        return Result.Success(order.ToOrderDetailDto());
    }
}
