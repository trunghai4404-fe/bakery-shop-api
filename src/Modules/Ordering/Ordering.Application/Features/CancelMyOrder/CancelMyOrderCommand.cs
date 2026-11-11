using MediatR;
using Ordering.Domain.Enums;
using Ordering.Domain.Errors;
using Ordering.Domain.Repositories;
using SharedKernel.Abstractions;
using SharedKernel.Domain;
using SharedKernel.Domain.Errors;

namespace Ordering.Application.Features.CancelMyOrder;

public record CancelMyOrderCommand(
    string OrderCode,
    string Reason,
    Guid? UserId = null
) : IRequest<Result>;

public class CancelMyOrderCommandHandler(
    IOrderRepository orderRepository,
    ICurrentUserService currentUserService,
    IOrderingUnitOfWork unitOfWork
) : IRequestHandler<CancelMyOrderCommand, Result>
{
    public async Task<Result> Handle(CancelMyOrderCommand request, CancellationToken ct)
    {
        var userId = request.UserId ?? currentUserService.UserId;
        if (!userId.HasValue)
        {
            return Result.Failure(Error.Unauthorized);
        }

        var order = await orderRepository.GetByCodeAndUserIdAsync(request.OrderCode, userId.Value, ct);
        if (order == null)
        {
            return Result.Failure(OrderingErrors.NotFoundByCode(request.OrderCode));
        }

        var cancelResult = order.CancelByCustomer(string.IsNullOrWhiteSpace(request.Reason) ? "Khách hàng yêu cầu hủy đơn" : request.Reason.Trim());
        if (!cancelResult.IsSuccess)
        {
            return cancelResult;
        }

        orderRepository.Update(order);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
