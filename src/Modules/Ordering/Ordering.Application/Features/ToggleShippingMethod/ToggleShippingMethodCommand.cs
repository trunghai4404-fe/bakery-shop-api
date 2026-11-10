using MediatR;
using Ordering.Contracts.DTOs;
using Ordering.Domain.Errors;
using Ordering.Domain.Repositories;
using SharedKernel.Domain;

namespace Ordering.Application.Features.ToggleShippingMethod;

public record ToggleShippingMethodCommand(
    string Code,
    bool? IsEnabled = null
) : IRequest<Result<ToggleShippingMethodResponse>>;

public class ToggleShippingMethodCommandHandler(
    IShippingMethodRepository shippingMethodRepository,
    IOrderingUnitOfWork unitOfWork
) : IRequestHandler<ToggleShippingMethodCommand, Result<ToggleShippingMethodResponse>>
{
    public async Task<Result<ToggleShippingMethodResponse>> Handle(
        ToggleShippingMethodCommand request,
        CancellationToken ct)
    {
        var method = await shippingMethodRepository.GetByIdOrCodeAsync(request.Code, ct);
        if (method is null)
        {
            return Result.Failure<ToggleShippingMethodResponse>(OrderingErrors.ShippingMethodNotFound(request.Code));
        }

        if (request.IsEnabled.HasValue)
        {
            method.UpdateStatus(request.IsEnabled.Value);
        }
        else
        {
            method.ToggleStatus();
        }

        shippingMethodRepository.Update(method);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new ToggleShippingMethodResponse(method.Code, method.IsEnabled));
    }
}
