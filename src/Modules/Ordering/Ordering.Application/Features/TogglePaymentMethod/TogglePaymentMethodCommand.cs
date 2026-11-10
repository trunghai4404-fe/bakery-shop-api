using MediatR;
using Ordering.Contracts.DTOs;
using Ordering.Domain.Errors;
using Ordering.Domain.Repositories;
using SharedKernel.Domain;

namespace Ordering.Application.Features.TogglePaymentMethod;

public record TogglePaymentMethodCommand(
    string Code,
    bool? IsEnabled = null
) : IRequest<Result<TogglePaymentMethodResponse>>;

public class TogglePaymentMethodCommandHandler(
    IPaymentMethodRepository paymentMethodRepository,
    IOrderingUnitOfWork unitOfWork
) : IRequestHandler<TogglePaymentMethodCommand, Result<TogglePaymentMethodResponse>>
{
    public async Task<Result<TogglePaymentMethodResponse>> Handle(
        TogglePaymentMethodCommand request,
        CancellationToken ct)
    {
        var method = await paymentMethodRepository.GetByIdOrCodeAsync(request.Code, ct);
        if (method is null)
        {
            return Result.Failure<TogglePaymentMethodResponse>(OrderingErrors.PaymentMethodNotFound(request.Code));
        }

        if (request.IsEnabled.HasValue)
        {
            method.UpdateStatus(request.IsEnabled.Value);
        }
        else
        {
            method.ToggleStatus();
        }

        paymentMethodRepository.Update(method);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new TogglePaymentMethodResponse(method.Code, method.IsEnabled));
    }
}
