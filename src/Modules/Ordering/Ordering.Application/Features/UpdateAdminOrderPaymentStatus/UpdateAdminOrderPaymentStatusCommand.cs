using MediatR;
using Ordering.Application.Common.Mappings;
using Ordering.Contracts.DTOs;
using Ordering.Domain.Enums;
using Ordering.Domain.Errors;
using Ordering.Domain.Repositories;
using SharedKernel.Domain;

namespace Ordering.Application.Features.UpdateAdminOrderPaymentStatus;

public record UpdateAdminOrderPaymentStatusCommand(
    long Id,
    string PaymentStatus
) : IRequest<Result<OrderDto>>;

public class UpdateAdminOrderPaymentStatusCommandHandler(
    IOrderRepository orderRepository,
    IOrderingUnitOfWork unitOfWork
) : IRequestHandler<UpdateAdminOrderPaymentStatusCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(UpdateAdminOrderPaymentStatusCommand request, CancellationToken ct)
    {
        var order = await orderRepository.GetByIdAsync(request.Id, ct);
        if (order == null)
        {
            return Result.Failure<OrderDto>(OrderingErrors.NotFound(request.Id));
        }

        if (!Enum.TryParse<PaymentStatus>(request.PaymentStatus, true, out var newPaymentStatus))
        {
            return Result.Failure<OrderDto>(OrderingErrors.InvalidPaymentStatus(request.PaymentStatus));
        }

        order.UpdatePaymentStatus(newPaymentStatus);
        orderRepository.Update(order);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(order.ToOrderDto());
    }
}
