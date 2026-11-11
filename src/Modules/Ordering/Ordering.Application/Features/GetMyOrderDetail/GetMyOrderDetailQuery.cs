using MediatR;
using Ordering.Application.Common.Mappings;
using Ordering.Contracts.DTOs;
using Ordering.Domain.Entities;
using Ordering.Domain.Errors;
using Ordering.Domain.Repositories;
using SharedKernel.Abstractions;
using SharedKernel.Domain;
using SharedKernel.Domain.Errors;

namespace Ordering.Application.Features.GetMyOrderDetail;

public record GetMyOrderDetailQuery(
    string OrderCode,
    Guid? UserId = null
) : IRequest<Result<OrderDto>>;

public class GetMyOrderDetailQueryHandler(
    IOrderRepository orderRepository,
    ICurrentUserService currentUserService
) : IRequestHandler<GetMyOrderDetailQuery, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(GetMyOrderDetailQuery request, CancellationToken ct)
    {
        Order? order;

        // Tự động phân quyền:
        // - Admin: Có thể xem chi tiết bất kỳ đơn hàng nào theo OrderCode
        // - User: Bắt buộc chỉ được xem đơn hàng của chính tài khoản mình
        if (currentUserService.IsAdmin)
        {
            order = await orderRepository.GetByCodeAsync(request.OrderCode, ct);
        }
        else
        {
            var userId = request.UserId ?? currentUserService.UserId;
            if (!userId.HasValue)
            {
                return Result.Failure<OrderDto>(Error.Unauthorized);
            }

            order = await orderRepository.GetByCodeAndUserIdAsync(request.OrderCode, userId.Value, ct);
        }

        if (order == null)
        {
            return Result.Failure<OrderDto>(OrderingErrors.NotFoundByCode(request.OrderCode));
        }

        return Result.Success(order.ToOrderDto());
    }
}
