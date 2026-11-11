using MediatR;
using Ordering.Application.Common.Mappings;
using Ordering.Contracts.DTOs;
using Ordering.Domain.Enums;
using Ordering.Domain.Errors;
using Ordering.Domain.Repositories;
using SharedKernel.Domain;

namespace Ordering.Application.Features.UpdateAdminOrderStatus;

public record UpdateAdminOrderStatusCommand(
    long Id,
    string Status,
    string? CancelReason = null
) : IRequest<Result<OrderDto>>;

public class UpdateAdminOrderStatusCommandHandler(
    IOrderRepository orderRepository,
    IOrderingUnitOfWork unitOfWork
) : IRequestHandler<UpdateAdminOrderStatusCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(UpdateAdminOrderStatusCommand request, CancellationToken ct)
    {
        var order = await orderRepository.GetByIdAsync(request.Id, ct);
        if (order == null)
        {
            return Result.Failure<OrderDto>(OrderingErrors.NotFound(request.Id));
        }

        if (!Enum.TryParse<OrderStatus>(request.Status, true, out var newStatus))
        {
            return Result.Failure<OrderDto>(OrderingErrors.InvalidStatus(request.Status));
        }

        var updateResult = order.UpdateOrderStatus(newStatus, request.CancelReason);
        if (!updateResult.IsSuccess)
        {
            return Result.Failure<OrderDto>(updateResult.Error);
        }

        orderRepository.Update(order);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(order.ToOrderDto());
    }
}
